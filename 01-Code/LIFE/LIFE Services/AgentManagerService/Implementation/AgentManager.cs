//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 18.12.2015
//  *******************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AgentManager.Interface;
using AgentManager.Interface.Exceptions;
using LifeAPI.Layer;
using MARS.Shuttle.SimulationConfig;
using SpatialAPI.Environment;
using ConfigService;
using System.Threading.Tasks;
using System.Text;
using LifeAPI.Agent;
using LCConnector.TransportTypes;
using MySql.Data.MySqlClient;
using CommonTypes;
using GeoGridEnvironment.Interface;
using DalskiAgent.Agents;
using AgentManager;

namespace AgentManagerService.Implementation
{
    public class AgentManager<T> : IAgentManager<T> where T : IAgent
    {
        public IDictionary<Guid, T> GetAgentsByAgentInitConfig(AgentInitConfig agentInitConfig, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle, List<ILayer> additionalLayerDependencies, IEnvironment environment = null, IGeoGridEnvironment<GpsAgent> geoGridEnvironment = null, int reducedAgentCount=-1)
        {
			Console.WriteLine ("Starting creation of agent type: " + agentInitConfig.AgentName);

            var agents = new ConcurrentDictionary<Guid, T>();
            var agentParameterCount = agentInitConfig.AgentInitParameters.Count;

			// connect to MARS ROCK
			// create ConfigService and connect to marsconfig container. This is due to convention. This LIFE container
			// should be linked to the marsconfig container and thus marsconfig should lead to the correct ip
			// as per /etc/hosts

			var marsConfigService = new ConfigServiceClient(MARSConfigServiceSettings.Address);
			Console.WriteLine($"AgentManager: Using {MARSConfigServiceSettings.Address} as MARSConfigService address.");
			// retreive ip, port, user and password of mariaDB to us as ROCK instance
			//string rockIp = marsConfigService.Get("rock/ip");
			//int rockPort = int.Parse(marsConfigService.Get("rock/port"));
			string rockIP = marsConfigService.Get("rock/ip");
			string rockUser = marsConfigService.Get("rock/serveruser");
			string rockPassword = marsConfigService.Get("rock/serverpassword");

			var connectionString = string.Format("Server={0};Uid={1};Pwd={2};",rockIP,rockUser,rockPassword);



            // Hook into the assembly resovle process, to load any neede .dll from Visual Studios' output directory
            // This needed when types need to be dynamically loaded by a De-Serializer and this code gets called from node.js/edge.js.
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolverFix.HandleAssemblyResolve;

            // retrieve agent constructor
            var agentType = Type.GetType(agentInitConfig.AgentFullName);

            var agentConstructor = agentType.GetConstructors().
                FirstOrDefault(c => c.GetCustomAttributes(typeof(PublishInShuttleAttribute), true).Length > 0);

			// fetch needed params
			var neededParameters = agentConstructor.GetParameters ();

            // sanity check for really needed parameters. Every parameter which has a default value might as well be 
            // initialized from that default value in case no mapping is present
            if (neededParameters.Length - neededParameters.Count(p => p.HasDefaultValue) != agentParameterCount)
            {
                throw new NotEnoughParametersProvidedException("There were not enough parameters provided in your SimConfig for Agent of type: " + agentType);
            }


            // setup arrays for agent parameters
			var agentDBParamArrays = new ConcurrentDictionary<string, string[]>();

			var initParams = agentInitConfig.AgentInitParameters;

			Parallel.ForEach (initParams, param => {
			    if (param.GetParameterType() !=
			        AtConstructorParameter.AtConstructorParameterType
			            .MarsCubeFieldToConstructorArgumentRelation) return;

			    var initInfo = param.GetFieldToConstructorArgumentRelation ();

			    // check if we already have this enumerator
			    if (agentDBParamArrays.ContainsKey(initInfo.MarsDBColumnName)) return;

				var sqlQuery = string.Format ("SELECT {0} FROM imports.{1} LIMIT {2} OFFSET {3}", initInfo.MarsDBColumnName, initInfo.MarsTableName, agentInitConfig.RealAgentCount, agentInitConfig.AgentInitOffset);
			    using (var mysqlConnection = new MySqlConnection(connectionString))
			    {
			        mysqlConnection.Open();
			        var cmd = new MySqlCommand(sqlQuery, mysqlConnection);
			        var reader = cmd.ExecuteReader();
			        var values = new List<string>();
			        while (reader.Read())
			        {
			            values.Add(reader.GetString(initInfo.MarsDBColumnName));
			        }
			        reader.Close();
					mysqlConnection.Close();
			        agentDBParamArrays.TryAdd(initInfo.MarsDBColumnName, values.ToArray());
			    }
			});

			Console.WriteLine ("Finished fetching DB data, Starting agent ID creation....");

            // get types for special parameters
            var layerType = typeof (ILayer);
            var guidType = typeof (Guid);
            var environmentType = typeof (IEnvironment);
            var geoGridEnvironmentType = typeof(IGeoGridEnvironment<GpsAgent>);
            var registerAgentType = typeof (RegisterAgent);
            var unregisterAgentType = typeof (UnregisterAgent);

			// setup agent count
			var agentCount = agentInitConfig.RealAgentCount;
			if (reducedAgentCount > 0 && reducedAgentCount <= agentInitConfig.RealAgentCount) {
				agentCount = reducedAgentCount;
			}


			var agentIds = new Guid[agentCount];

			Parallel.For(0, agentCount, i => agentIds[i] = Guid.NewGuid ());
			Console.WriteLine ("Finished agent ID creation, Starting agent creation.... AgentCount is : {0}", agentCount);

			// iterate over all agents and create them
			Parallel.For (0, agentCount, index => {

				// create Agent ID
				var realAgentId = agentIds[index];

				// the list which will hold the actual Parameters
				var actualParameters = new List<object> ();

				// get an enumerator for the parameters provided by SHUTTLE
                var shuttleParams = initParams.GetEnumerator();
                var nextParamAvailable = shuttleParams.MoveNext();

                foreach (var neededParam in neededParameters) {

					// check special types
					if (environmentType.IsAssignableFrom (neededParam.ParameterType)) {
						actualParameters.Add(environment);
                    } else if(geoGridEnvironmentType.IsAssignableFrom(neededParam.ParameterType)) {
                        actualParameters.Add(geoGridEnvironment);
                    } else if (layerType.IsAssignableFrom (neededParam.ParameterType)) {
						if (!additionalLayerDependencies.Any (l => neededParam.ParameterType.IsInstanceOfType (l))) {
							throw new MissingLayerForAgentConstructionException ("Agent type '" + agentInitConfig.AgentName + "' needs missing layer type '"
							+ neededParam.ParameterType + "' to initialize.");
						}
						actualParameters.Add (additionalLayerDependencies.First (l => neededParam.ParameterType.IsInstanceOfType (l)));
					} else if (guidType.IsAssignableFrom (neededParam.ParameterType)) {
						actualParameters.Add (realAgentId);      
					} else if (registerAgentType.IsAssignableFrom (neededParam.ParameterType)) {
						actualParameters.Add (registerAgentHandle);
					} else if (unregisterAgentType.IsAssignableFrom (neededParam.ParameterType)) {
						actualParameters.Add (unregisterAgentHandle);
					} else {

                        // check whether a next parameter is avialable from SHUTTLE:
                        if (nextParamAvailable)
                        {
                            // it's a primitive type, so take the next param from params list provided by SHUTTLE
                            var param = shuttleParams.Current;

                            if (param.GetParameterType() == AtConstructorParameter.AtConstructorParameterType.ConstantParameterToConstructorArgumentRelation)
                            {
                                // use static value
                                var initInfo = param.GetConstantParameterToConstructorArgumentRelation();
                                var paramType = Type.GetType(initInfo.ConstructorArgumentDatatype);

                                if (paramType != typeof(string) && (paramType == null || !paramType.IsPrimitive))
                                {
                                    throw new ParameterMustBePrimitiveException("The parameter " + initInfo.ConstructorArgumentName + " must be a primitive C# type. But was: " + paramType.Name);
                                }

                                try
                                {
                                    actualParameters.Add(GetParameterValue(paramType, initInfo.ParameterValue));
                                }
                                catch (FormatException formatException)
                                {
                                    Console.Error.WriteLine("An error occured while transforming a value" +
                                    " from ROCK-DB. " +
                                    "The destined target type is: {0} ," +
                                    " the value field contained: {1}," +
                                    " the argument name was: {2}, " +
                                    " the original exception was: {3}."
                                        , paramType, initInfo.ParameterValue, initInfo.ConstructorArgumentName, formatException);
                                    throw formatException;
                                }
                            }

                            if (param.GetParameterType() == AtConstructorParameter.AtConstructorParameterType.MarsCubeFieldToConstructorArgumentRelation)
                            {
                                var initInfo = param.GetFieldToConstructorArgumentRelation();
                                var paramType = Type.GetType(initInfo.ConstructorArgumentDatatype);


                                // fetch parameter from ROCK CUBE
                                var paramValue = agentDBParamArrays[initInfo.MarsDBColumnName][index];
                                // add param to actualParameters[]
                                try
                                {
                                    actualParameters.Add(GetParameterValue(paramType, paramValue));
                                }
                                catch (FormatException formatException)
                                {
                                    Console.Error.WriteLine("An error occured while transforming a value" +
                                    " from ROCK-DB. " +
                                    "The destined target type is: {0} ," +
                                    " the value field contained: {1}," +
                                    " the argument name was: {2}, " +
                                    " the original exception was: {3}."
                                        , paramType, paramValue, initInfo.ConstructorArgumentName, formatException);
                                    throw formatException;
                                }
                            }
                        }
                        // no next param avialable, but still params are needed, these must be params with default values
                        else {
                            if (!neededParam.HasDefaultValue)
                            {
                                throw new ParameterIsNotMappedOrHasNotDefaultValue($"The parameter {neededParam.Name} was not mapped in SHUTTLE or has no default value assigned!"); 
                            }
                            actualParameters.Add(neededParam.DefaultValue);
                        }

					}

                    // move to next param
                    nextParamAvailable = shuttleParams.MoveNext();

                }

				// call constructor of agent and store agent in return dictionary
				try {
					agents.TryAdd (realAgentId, (T)agentConstructor.Invoke (actualParameters.ToArray()));
				} catch(TargetParameterCountException tex) {
					var stb = new StringBuilder();
					actualParameters.Select(p => stb.Append(p.GetType().Name + "/n"));
					Console.Error.WriteLine("The amount of provided parameters doesn fit with the amount of required parameters. Parameters were: " + stb);
					throw tex;
				}
			});

			Console.WriteLine (String.Format("Finished agent creation. Created {0} agents.", agentCount));
            return agents;
        }

        /// <summary>
        /// Transforms the JSON string values to actual C# primitive types
        /// </summary>
        /// <param name="parameterDatatype"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        private static object GetParameterValue(Type parameterDatatype, string parameterValue) {
	            var provider = new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
	            if (parameterDatatype == typeof (double)) {
	                return Convert.ToDouble(parameterValue, provider);
	            }

	            if (parameterDatatype == typeof(int))
	            {
	                return int.Parse(parameterValue);
	            }

	            if (parameterDatatype == typeof(bool))
	            {
	                return bool.Parse(parameterValue);
	            }

	            if (parameterDatatype == typeof(float))
	            {
	                return float.Parse(parameterValue);
	            }

	            if (parameterDatatype == typeof(char))
	            {
	                return char.Parse(parameterValue);
	            }

	            // string is returned by default down below, but to save some if-checks, do it here too
	            if (parameterDatatype == typeof(string))
	            {
	                return parameterValue;
	            }

	            if (parameterDatatype == typeof(long))
	            {
	                return long.Parse(parameterValue);
	            }

	            if (parameterDatatype == typeof(byte))
	            {
	                return byte.Parse(parameterValue);
	            }

	            if (parameterDatatype == typeof(sbyte))
	            {
	                return sbyte.Parse(parameterValue);
	            }

	            if (parameterDatatype == typeof(uint))
	            {
	                return uint.Parse(parameterValue);
	            }

	            if (parameterDatatype == typeof(short))
	            {
	                return short.Parse(parameterValue);
	            }

	            if (parameterDatatype == typeof(ushort))
	            {
	                return ushort.Parse(parameterValue);
	            }

	            if (parameterDatatype == typeof(ulong))
	            {
	                return ulong.Parse(parameterValue);
	            }

	            if (parameterDatatype == typeof(decimal))
	            {
	                return decimal.Parse(parameterValue);
	            }

            return parameterValue;
        }

        /// <summary>
        /// Resolves an issue which occurs when an assembly should be loaded triggered by an external process.
        /// By default the assembly is only being searched for in the current context and not in all
        /// currently loaded assemblies. This is fixed here.
        /// </summary>
        private static class AssemblyResolverFix
        {
            //Looks up the assembly in the set of currently loaded assemblies,
            //and returns it if the name matches. Else returns null.
            public static Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args)
            {
                return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(ass => ass.FullName == args.Name);
            }
        }
    }
}
