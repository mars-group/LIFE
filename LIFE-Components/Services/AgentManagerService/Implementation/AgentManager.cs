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
using System.Text;
using System.Threading.Tasks;
using LIFE.API.Agent;
using LIFE.API.Environment;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using LIFE.API.LIFECapabilities;
using LIFE.Components.Services.AgentManagerService.Interface.Exceptions;

namespace LIFE.Components.Services.AgentManagerService.Implementation
{
    public static class AgentManager
    {
        public static IDictionary<Guid, T> GetAgentsByAgentInitConfig<T>(AgentInitConfig agentInitConfig,
            RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle,
            List<ILayer> additionalLayerDependencies,
            IEnvironment environment = null, int reducedAgentCount = -1) where T : IAgent
        {
            Console.WriteLine("[AM] Starting creation of agent type: " + agentInitConfig.AgentName);

            var agents = new ConcurrentDictionary<Guid, T>();
            var agentParameterCount = agentInitConfig.AgentInitParameters.Count;


            // retrieve agent constructor
            var agentType = Type.GetType(agentInitConfig.AgentFullName);

            var agentConstructor = agentType.GetTypeInfo()
                .GetConstructors()
                .FirstOrDefault(c => c.GetCustomAttributes(typeof(PublishForMappingInMarsAttribute), true).Any());
            if (agentConstructor == null)
            {
                var msg =
                    "AgentConstructor was not found. Either Reflection is not working or you forgot to annotate a ctor with [PublishForMappingInMars]";
                Console.WriteLine(msg);
                throw new AgentConstructorWasNullException("msg");
            }
            // fetch needed params
            var neededParameters = agentConstructor.GetParameters();

            // sanity check for really needed parameters. Every parameter which has a default value might as well be
            // initialized from that default value in case no mapping is present
            if (neededParameters.Length - neededParameters.Count(p => p.HasDefaultValue) != agentParameterCount)
            {
                var errmsg = new StringBuilder();
                errmsg.AppendLine(
                    "There were not enough parameters provided in your SimConfig for AgentReference of type: " +
                    agentType);
                errmsg.Append("NeededParams are:");
                foreach (var np in neededParameters) errmsg.AppendLine($"Name: {np.Name}, Type: {np.GetType()}");
                errmsg.Append("ActualParams are:");
                foreach (var np in agentInitConfig.AgentInitParameters)
                    errmsg.AppendLine($"Type: {np.Type}");
                Console.WriteLine(errmsg.ToString());
                throw new NotEnoughParametersProvidedException(errmsg.ToString());
            }


            // setup arrays for agent parameters
            var agentParameterFetcher = new AgentParameterFetcher();
            var agentDbParamArrays =
                agentParameterFetcher.GetParametersForInitConfig(agentInitConfig);

            var initParamsFromMARSCloud = agentInitConfig.AgentInitParameters;


            Console.WriteLine ("[AM] Finished fetching DB data, Starting agent ID creation....");

            // get types for special parameters
            var layerType = typeof(ILayer);
            var guidType = typeof(Guid);
            var registerAgentType = typeof(RegisterAgent);
            var unregisterAgentType = typeof(UnregisterAgent);

            // setup agent count
            var agentCount = agentInitConfig.RealAgentCount;
            if ((reducedAgentCount > 0) && (reducedAgentCount <= agentInitConfig.RealAgentCount))
                agentCount = reducedAgentCount;


            var agentIds = new Guid[agentCount];

            Parallel.For(0, agentCount, i => agentIds[i] = Guid.NewGuid());
            Console.WriteLine ("[AM] Finished agent ID creation, Starting agent creation.... AgentCount is : {0}", agentCount);

            // iterate over all agents and create them
            Parallel.For(0L, agentCount, index =>
            {
                // create AgentReference ID
                var realAgentId = agentIds[index];

                // the list which will hold the actual Parameters
                var actualParameters = new List<object>();


                foreach (var neededParam in neededParameters)
                {
                    // check special types
                    if (neededParam.ParameterType.GetTypeInfo().IsInstanceOfType(environment))
                    {
                        actualParameters.Add(environment);
                    }
                    else if (layerType.GetTypeInfo().IsAssignableFrom(neededParam.ParameterType))
                    {
                        if (!additionalLayerDependencies.Any(l => neededParam.ParameterType.GetTypeInfo()
                            .IsInstanceOfType(l)))
                            throw new MissingLayerForAgentConstructionException(
                                "AgentReference type '" + agentInitConfig.AgentName +
                                "' needs missing layer type '"
                                + neededParam.ParameterType + "' to initialize.");
                        actualParameters.Add(
                            additionalLayerDependencies.First(l => neededParam.ParameterType.GetTypeInfo()
                                .IsInstanceOfType(l)));
                    }
                    else if (guidType.GetTypeInfo().IsAssignableFrom(neededParam.ParameterType))
                    {
                        actualParameters.Add(realAgentId);
                    }
                    else if (registerAgentType.GetTypeInfo().IsAssignableFrom(neededParam.ParameterType))
                    {
                        actualParameters.Add(registerAgentHandle);
                    }
                    else if (unregisterAgentType.GetTypeInfo().IsAssignableFrom(neededParam.ParameterType))
                    {
                        actualParameters.Add(unregisterAgentHandle);
                    }
                    else
                    {
                        // check whether a parameter with correct name is available from MARS Cloud Mapping:
                        if (initParamsFromMARSCloud.Any(e => e.Name.Equals(neededParam.Name)))
                        {

                            // it's a primitive type, so take the matching param from params list provided by MARS Cloud Mapping
                            var param = initParamsFromMARSCloud.First(e => e.Name.Equals(neededParam.Name));

                            if (param.MappingType == MappingType.ValueParameterMapping)
                            {
                                // use static value
                                var paramType = neededParam.ParameterType;

                                if (paramType != typeof(string) && !paramType.GetTypeInfo().IsEnum && (paramType == null || !paramType.GetTypeInfo().IsPrimitive))
                                {
                                    throw new ParameterMustBePrimitiveException("The parameter " + param.Name + " must be a primitive C# type. But was: " + paramType.Name);
                                }

                                try
                                {
                                    actualParameters.Add(GetParameterValue(paramType, param.Value));
                                }
                                catch (FormatException formatException)
                                {
                                    Console.Error.WriteLine("An error occured while transforming a value" +
                                                            " from ROCK-DB. " +
                                                            "The destined target type is: {0} ," +
                                                            " the value field contained: {1}," +
                                                            " the argument name was: {2}, " +
                                                            " the original exception was: {3}."
                                        , paramType, param.Value, param.Name, formatException);
                                    throw formatException;
                                }
                            }

                            if (param.MappingType == MappingType.ColumnParameterMapping)
                            {
                                var paramType = neededParam.ParameterType;


                                // fetch parameter from ROCK CUBE
                                var paramValue = agentDbParamArrays[param.ColumnName][index];
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
                                        , paramType, paramValue, param.Name, formatException);
                                    throw formatException;
                                }
                            }
                        }
                        // no next param available, but still params are needed, these must be params with default values
                        else
                        {
                            if (!neededParam.HasDefaultValue)
                            {
                                throw new ParameterIsNotMappedOrHasNotDefaultValue(
                                    $"The parameter {neededParam.Name} was not mapped in SHUTTLE or has no default value assigned!");
                            }

                            Console.WriteLine("Hit8");
                            actualParameters.Add(neededParam.DefaultValue);

                        }

                    }

                }


                // call constructor of agent and store agent in return dictionary
                int paramCounter = 0;
                foreach (var actualParameter in actualParameters)
                {
                    if (actualParameter == null)
                    {
                        throw new ActualParameterNullPointerException($"Parameter {neededParameters[paramCounter].Name} was null after init.");
                    }
                    paramCounter++;
                }

                try
                {
                    agents.TryAdd(realAgentId, (T) agentConstructor.Invoke(actualParameters.ToArray()));
                }
                catch (TargetParameterCountException tex)
                {
                    var stb = new StringBuilder();
                    actualParameters.Select(p => stb.Append(p.GetType().Name + "/n"));
                    Console.Error.WriteLine(
                        "The amount of provided parameters doesn fit with the amount of required parameters. Parameters were: " +
                        stb);
                    throw tex;
                }
            });

            Console.WriteLine (String.Format("[AM] Finished agent creation. Created {0} agents.", agentCount));

            return agents;
        }

        /// <summary>
        ///   Transforms the JSON string values to actual C# primitive types
        /// </summary>
        /// <param name="parameterDatatype"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        private static object GetParameterValue(Type parameterDatatype, string parameterValue)
        {
            var provider = new NumberFormatInfo {NumberDecimalSeparator = ".", NumberGroupSeparator = ","};
            if (parameterDatatype.GetTypeInfo().IsEnum)
            {
                Console.WriteLine(parameterDatatype.FullName);
                Console.WriteLine($"Value: {parameterValue}");

                var e = Enum.Parse(parameterDatatype, parameterValue, true);
                if (e == null)
                {
                    throw new FormatException(
                        $"Enum {parameterDatatype.Name} could not be parsed! Value was: {parameterValue}");
                }
                return e;
            }
            if (parameterDatatype == typeof(double))
            {
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
    }
}