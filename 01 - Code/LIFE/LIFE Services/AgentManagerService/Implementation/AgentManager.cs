// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 06.11.2014
//  *******************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using AgentManager.Interface;
using AgentManager.Interface.Exceptions;
using LCConnector.TransportTypes;
using LifeAPI.Agent;
using LifeAPI.Layer;
using mars.rock.drill;
using MARS.Shuttle.SimulationConfig;
using SpatialAPI.Environment;


namespace AgentManagerService.Implementation
{
    public class AgentManager<T> : IAgentManager<T> where T : IAgent
    {
        public IDictionary<Guid, T> GetAgentsByAgentInitConfig(AgentInitConfig agentInitConfig, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle, IEnvironment environment, List<ILayer> additionalLayerDependencies)
        {
            var agents = new ConcurrentDictionary<Guid, T>();
            var agentParameterCount = agentInitConfig.AgentInitParameters.Count;

            // connect to MARS ROCK
            // agentInitConfig.MarsCubeUrl
            Drill.InitializeConnection("141.22.29.9", "mars", "82cxhpcqA5SEHdcikmbx");

            var initParams = agentInitConfig.AgentInitParameters;

            // Hook into the assembly resovle process, to load any neede .dll from Visual Studios' output directory
            // This needed when types need to be dynamically loaded by a De-Serializer and this code gets called from node.js/edge.js.
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolverFix.HandleAssemblyResolve;

            // retrieve agent constructor
            var agentType = Type.GetType(agentInitConfig.AgentFullName);

            var agentConstructor = agentType.GetConstructors().
                FirstOrDefault(c => c.GetCustomAttributes(typeof(PublishInShuttleAttribute), true).Length > 0);

            // sanity check
            if (agentConstructor.GetParameters().Length != agentParameterCount)
            {
                throw new NotEnoughParametersProvidedException("There were not enough parameters provided in your SimConfig for Agent of type: " + agentType);
            }

            // setup enumerators for cube parameters
            var agentCubeParamEnumerators = new Dictionary<string, IEnumerator<object>>();
            foreach (var param in initParams)
            {

                if (param.GetParameterType()
                    == AtConstructorParameter.AtConstructorParameterType.MarsCubeFieldToConstructorArgumentRelation)
                {
                    var initInfo = param.GetMarsCubeFieldToConstructorArgumentRelation();

                    // check if we already have this enumerator
                    if (!agentCubeParamEnumerators.ContainsKey(initInfo.MarsCubeDBColumnName))
                    {
                        // call to Drill API
                        var cube = Drill.GetCube(agentInitConfig.MarsCubeName);

                        // fetch needed dimension from cube
                        var data = cube.GetData
                            (new List<Dimension> {
                                    cube.Dimensions.FirstOrDefault
                                        (d =>
                                            d.CleanName == initInfo.MarsCubeDimensionName
                                            || d.Name == initInfo.MarsCubeDimensionName)
                                });

                        // get real column name from CleanName
                        var columnName = cube.Dimensions.FirstOrDefault
                            (d =>
                                d.CleanName == initInfo.MarsCubeDimensionName
                                || d.Name == initInfo.MarsCubeDimensionName)
                            .Attributes.FirstOrDefault(a => a.CleanName == initInfo.MarsCubeDBColumnName)
                            .Name;

                        // create enumerators for data retrieval
                        agentCubeParamEnumerators.Add
                            (initInfo.MarsCubeDBColumnName,
                                (from DataRow dr in data.Rows
                                 select dr[columnName]).GetEnumerator());
                        // set enum to first element
                        agentCubeParamEnumerators[initInfo.MarsCubeDBColumnName].MoveNext();
                        // advance to first real data element
                        while (agentCubeParamEnumerators[initInfo.MarsCubeDBColumnName].Current is DBNull) {
                            agentCubeParamEnumerators[initInfo.MarsCubeDBColumnName].MoveNext();
                        }
                    }
                }
            }

            // get types
            var layerType = typeof (ILayer);
            var guidType = typeof (Guid);
            var environmentType = typeof (IEnvironment);
            var registerAgentType = typeof (RegisterAgent);
            var unregisterAgentType = typeof (UnregisterAgent);

            foreach (var realAgentId in agentInitConfig.RealAgentIds) {
                var actualParameters = new List<object>(agentParameterCount);

				var paramEnumerator = initParams.GetEnumerator();
                
                // move enumerator to first element
                paramEnumerator.MoveNext();
                
                // fetch needed params
				var neededParameters = agentConstructor.GetParameters ();

				foreach (var neededParam in neededParameters) {
                    if (layerType.IsAssignableFrom(neededParam.ParameterType)) {
				        actualParameters.Add(additionalLayerDependencies.First(l => neededParam.ParameterType.IsInstanceOfType(l)));
				    } else if (guidType.IsAssignableFrom(neededParam.ParameterType)) {
				        actualParameters.Add(realAgentId);      
				    } else if (environmentType.IsAssignableFrom(neededParam.ParameterType)) {
				        actualParameters.Add(environment);
				    } else if (registerAgentType.IsAssignableFrom(neededParam.ParameterType)) {
				        actualParameters.Add(registerAgentHandle);
                    } else if (unregisterAgentType.IsAssignableFrom(neededParam.ParameterType)) {
				        actualParameters.Add(unregisterAgentHandle);
				    } else {
				    // it's a primitive type, so take the next param from params list provided by SHUTTLE
						var param = paramEnumerator.Current;

						if (param.GetParameterType() == AtConstructorParameter.AtConstructorParameterType.ConstantParameterToConstructorArgumentRelation)
						{
							// use static value
							var initInfo = param.GetConstantParameterToConstructorArgumentRelation();
							var paramType = Type.GetType(initInfo.ConstructorArgumentDatatype);

							if (paramType == null || !paramType.IsPrimitive) {
								throw new ParameterMustBePrimitiveException("The parameter " + initInfo.ConstructorArgumentName + " must be a primitive C# type.");

							}
							actualParameters.Add(GetParameterValue(paramType, initInfo.ParameterValue));
						}

						if (param.GetParameterType() == AtConstructorParameter.AtConstructorParameterType.MarsCubeFieldToConstructorArgumentRelation) {
							var initInfo = param.GetMarsCubeFieldToConstructorArgumentRelation();
                            var paramType = Type.GetType(initInfo.ConstructorArgumentDatatype);
							// fetch parameter from ROCK CUBE
							var paramValue = agentCubeParamEnumerators[initInfo.MarsCubeDBColumnName].Current;
                            // advance to first real data element
                            while (agentCubeParamEnumerators[initInfo.MarsCubeDBColumnName].Current is DBNull)
                            {
                                agentCubeParamEnumerators[initInfo.MarsCubeDBColumnName].MoveNext();
                            }
							// add param to actualParameters[]
                            actualParameters.Add(GetParameterValue(paramType, (string)paramValue));
						}   	
					}
                    // move shuttleParams to next element
                    paramEnumerator.MoveNext();
				}

                // call constructor of agent and store agent in return dictionary
                agents.TryAdd(realAgentId, (T)agentConstructor.Invoke(actualParameters.ToArray()));
            }

            return agents;
        }

        /// <summary>
        /// Transforms the JSON string values to actual C# primitive types
        /// </summary>
        /// <param name="parameterDatatype"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        private static object GetParameterValue(Type parameterDatatype, string parameterValue) {
            if (parameterDatatype == typeof (double)) {
                return Double.Parse(parameterValue);
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
