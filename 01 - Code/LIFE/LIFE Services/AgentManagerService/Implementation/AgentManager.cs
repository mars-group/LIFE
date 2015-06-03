using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        public Dictionary<Guid, T> GetAgentsByAgentInitConfig(AgentInitConfig agentInitConfig, IEnvironment environment, List<ILayer> additionalLayerDependencies)
        {
            var agents = new Dictionary<Guid, T>();
            var agentParameterCount = agentInitConfig.AgentInitParameters.Count;

            // connect to MARS ROCK
            Drill.InitializeConnection(agentInitConfig.MarsCubeUrl, "mars", "82cxhpcqA5SEHdcikmbx");

            var initParams = agentInitConfig.AgentInitParameters;

            // retrieve agent constructor
            var agentType = Type.GetType(agentInitConfig.AgentName);

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

                        var data = cube.GetData
                            (new List<Dimension> {
                                    cube.Dimensions.FirstOrDefault
                                        (d =>
                                            d.CleanName == initInfo.MarsCubeDimensionName
                                            || d.Name == initInfo.MarsCubeDimensionName)
                                });

                        agentCubeParamEnumerators.Add
                            (initInfo.MarsCubeDBColumnName,
                                (from DataRow dr in data.Rows
                                 select dr[initInfo.MarsCubeDBColumnName]).GetEnumerator());
                    }
                }
            }

            foreach (var realAgentId in agentInitConfig.RealAgentIds) {
                var actualParameters = new List<object>(agentParameterCount);
				var paramEnumerator = initParams.GetEnumerator ();
				var neededParameters = agentConstructor.GetParameters ();
				foreach (var neededParam in neededParameters) {
				    // check whether the parameter is an instance of ILayer
				    var layer = neededParam.ParameterType as ILayer;
				    var env = neededParam.ParameterType as IEnvironment;
				    if(layer != null){
						actualParameters.Add (additionalLayerDependencies.First(l => l.GetType () == neededParam.ParameterType));	
					} else if (env != null) {
					    actualParameters.Add(environment);
					} else {
				    // it's a primitive type, so take the next param from params list provided by SHUTTLE
						var param = paramEnumerator.Current;
						paramEnumerator.MoveNext();

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

						if (param.GetParameterType() ==
							AtConstructorParameter.AtConstructorParameterType.MarsCubeFieldToConstructorArgumentRelation) {
							var initInfo = param.GetMarsCubeFieldToConstructorArgumentRelation();
							// fetch parameter from ROCK CUBE
							var paramValue = agentCubeParamEnumerators[initInfo.MarsCubeDBColumnName].Current;
							// advance Enumerator
							agentCubeParamEnumerators[initInfo.MarsCubeDBColumnName].MoveNext();
							// add param to actualParameters[]
							actualParameters.Add(paramValue);
						}   	
					}
				}

                // call constructor of agent and store agent in return dictionary
                agents.Add(realAgentId, (T)agentConstructor.Invoke(actualParameters.ToArray()));
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
    }
}
