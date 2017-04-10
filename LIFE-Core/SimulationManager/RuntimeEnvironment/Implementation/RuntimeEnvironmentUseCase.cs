//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 18.12.2015
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommonTypes;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using Hik.Communication.ScsServices.Client;
using Hik.Communication.ScsServices.Communication.Messages;
using LayerContainerFacade.Interfaces;
using LCConnector;
using LIFE.API.Config;
using LIFE.API.Layer.Initialization;
using LIFE.API.Layer.Obstacle;
using LIFE.API.Layer.PotentialField;
using LIFE.API.Layer.TimeSeries;
using ModelContainer.Interfaces;
using Newtonsoft.Json.Linq;
using NodeRegistry.Interface;
using RuntimeEnvironment.Implementation.Entities;
using RuntimeEnvironment.Interfaces;
using SMConnector;
using SMConnector.Exceptions;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Implementation
{
    internal class RuntimeEnvironmentUseCase : IRuntimeEnvironment
    {
        private readonly IModelContainer _modelContainer;
        private readonly INodeRegistry _nodeRegistry;
        private readonly IDictionary<TModelDescription, SteppedSimulationExecutionUseCase> _steppedSimulations;
        private readonly ISet<TNodeInformation> _idleLayerContainers;
        private readonly ISet<TNodeInformation> _busyLayerContainers;
        private Guid _simulationId;

        public RuntimeEnvironmentUseCase
        (
            IModelContainer modelContainer,
            INodeRegistry nodeRegistry)
        {
            _modelContainer = modelContainer;
            _nodeRegistry = nodeRegistry;

            _steppedSimulations = new Dictionary<TModelDescription, SteppedSimulationExecutionUseCase>();
            _idleLayerContainers = new HashSet<TNodeInformation>();
            _busyLayerContainers = new HashSet<TNodeInformation>();

            // register for new LayerContainers
            _nodeRegistry.SubscribeForNewNodeConnectedByType(NewNode, NodeType.LayerContainer);
        }

        #region IRuntimeEnvironment Members

        public void StartWithModel(Guid simulationId, TModelDescription model,
            ICollection<TNodeInformation> layerContainerNodes,
            int? nrOfTicks = null, string scenarioConfigId = "", string resultConfigId = "", bool startPaused = false,
            ILayerContainerFacade layerContainer = null)
        {
            _simulationId = simulationId;

            if (layerContainer == null && (layerContainerNodes.Count <= 0 || _idleLayerContainers.Count <= 0))
            {
                throw new NoLayerContainersArePresentException();
            }

            Console.WriteLine("Found and working with " + layerContainerNodes.Count + " Layercontainers.");

            // if not all LayerContainers are idle throw exception
            if (!layerContainerNodes.All(l => _idleLayerContainers.Any(c => c.Equals(l))))
            {
                throw new LayerContainerBusyException();
            }

            Console.WriteLine("Setting up SimulationRun...");
            var sw = Stopwatch.StartNew();

            // try to get ScenarioConfig and determine various information from it
            var scenarioconfigJson = _modelContainer.GetScenarioConfig(scenarioConfigId);
            if (scenarioconfigJson != null)
            {
                var globalParams = scenarioconfigJson["ParameterizationDescription"]["Global"];
                var startDate = DateTime.Parse(globalParams["SimulationStartDateTime"].ToString());
                var endDate = DateTime.Parse(globalParams["SimulationEndDateTime"].ToString());
                var deltaT = int.Parse(globalParams["DeltaT"].ToString());
                var deltaTUnit = globalParams["DeltaTUnit"].ToString();
                var duration = (endDate - startDate);
                var simStepDuration = GetDeltaTUnitTimeSpan(deltaTUnit, deltaT);
                var tickCount = (int) (duration.TotalMilliseconds / simStepDuration.TotalMilliseconds);
                if (tickCount > 0)
                {
                    nrOfTicks = tickCount;
                }
            }

            model.Name = scenarioconfigJson["Name"].ToString();


            IList<LayerContainerClient> clients = SetupSimulationRun(model, layerContainerNodes, scenarioConfigId,
                resultConfigId, layerContainer);


            sw.Stop();
            Console.WriteLine("...done in " + sw.ElapsedMilliseconds + "ms or " + sw.Elapsed);

            _steppedSimulations[model] =
                new SteppedSimulationExecutionUseCase(nrOfTicks, clients, simulationId, startPaused);
        }

        public void StepSimulation(TModelDescription model, ICollection<TNodeInformation> layerContainerNodes,
            int? nrOfTicks = null)
        {
            if (!_steppedSimulations.ContainsKey(model))
            {
                throw new SimulationHasNotBeenStartedException
                (
                    "It appears that you did not start your simulation yet. Please call StartSimulationWithModel(...) first.");
            }
            _steppedSimulations[model].StepSimulation(nrOfTicks);
        }

        public void Pause(TModelDescription model)
        {
            if (!_steppedSimulations.ContainsKey(model))
            {
                throw new SimulationHasNotBeenStartedException
                (
                    "It appears that you did not start your simulation yet. Please call StartSimulationWithModel(...) first.");
            }

            _steppedSimulations[model].PauseSimulation();
        }

        public void Resume(TModelDescription model)
        {
            if (!_steppedSimulations.ContainsKey(model))
            {
                throw new SimulationHasNotBeenStartedException
                (
                    "It appears that you did not start your simulation yet. Please call StartSimulationWithModel(...) first.");
            }

            _steppedSimulations[model].ResumeSimulation();
        }

        public void Abort(TModelDescription model)
        {
            if (!_steppedSimulations.ContainsKey(model)) return;

            _steppedSimulations[model].Abort();
        }


        public void WaitForSimulationToFinish(TModelDescription model)
        {
            if (!_steppedSimulations.ContainsKey(model))
            {
                throw new SimulationHasNotBeenStartedException
                (
                    "It appears that you did not start your simulation yet. Please call StartSimulationWithModel(...) first.");
            }
            _steppedSimulations[model].WaitForSimulationToFinish();
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable)
        {
        }

        #endregion

        /// <summary>
        ///     Establishes connections to the given nodes and instantiates the model on all layer containers, according to the
        ///     instantiation order.
        /// </summary>
        /// <param name="modelDescription">not null</param>
        /// <param name="layerContainers">not null</param>
        /// <returns></returns>
        private LayerContainerClient[] SetupSimulationRun(TModelDescription modelDescription,
            ICollection<TNodeInformation> layerContainers, string simConfigName, string resultConfigId,
            ILayerContainerFacade layerContainer = null)
        {
            var content = _modelContainer.GetSerializedModel(modelDescription);
            var layerContainerClients = new List<LayerContainerClient>();
            Console.WriteLine("Creating LayerContainer Clients...");
            /* 1.
             * Create LayerContainerClients for all connected LayerContainers if no direct reference is passed
             */
            if (layerContainer != null)
            {
                layerContainerClients.Add(new LayerContainerClient(layerContainer, content, resultConfigId));
            }
            else
            {
                foreach (var nodeInformationType in layerContainers)
                {
                    var retries = 0;
                    var connected = false;
                    while (!connected && retries < 3)
                    {
                        try
                        {
                            var client = new LayerContainerClient(
                                ScsServiceClientBuilder.CreateClient<ILayerContainer>
                                (
                                    nodeInformationType.NodeEndpoint.IpAddress + ":" +
                                    nodeInformationType.NodeEndpoint.Port
                                ),
                                content, resultConfigId);
                            layerContainerClients.Add(client);
                            connected = true;
                        }
                        catch (ScsRemoteException scsEx)
                        {
                            Console.WriteLine(scsEx.StackTrace);

                            throw scsEx;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Layercontainer Connection ERROR: " + ex.Message);
                            // fail after 3 attempts
                            if (retries > 1)
                            {
                                var sockEx = ex as SocketException;
                                if (sockEx != null)
                                {
                                    Console.WriteLine(
                                        "A LayerContainer could not be connected. Continueing without it. Exception was: {0}",
                                        sockEx.Message);
                                }
                                else
                                {
                                    throw ex;
                                }
                            }
                            else
                            {
                                Thread.Sleep(500);
                                retries++;
                            }
                        }
                    }
                }
            }

            Console.WriteLine("...done!");

            /* Load configuration and determine which one to use. SHUTTLE files will be prefered, old-school
             * XML config files are still valid but will be deprecated in the near future
             */
            var modelConfig = _modelContainer.GetModelConfig(modelDescription);
            Console.WriteLine("Get Scenario Config...");
            var scenarioConfig = _modelContainer.GetScenarioConfig(simConfigName);


            // only accept ScenarioConfig based configuration
            if (scenarioConfig != null)
            {
                // configure via ScenarioConfig
                return SetupSimulationRunViaScenarioConfig(modelDescription, layerContainerClients.ToArray(),
                    scenarioConfig, modelConfig);
            }
            throw new Exception(
                "No ScenarioConfiguration has been found. Please use the --sc flag to provide the ID of a ScenarioConfiguration and make sure" +
                "you're running this simulation in a MARS Cloud instance!");
        }


        private LayerContainerClient[] SetupSimulationRunViaScenarioConfig(TModelDescription modelDescription,
            LayerContainerClient[] layerContainerClients, JObject scenarioConfig, ModelConfig modelConfig)
        {
            /* 2.
             * Instantiate and initialize Layers by InstantiationOrder,
             * For now don'tdifferentiate between distributable and non-distributable layers
             * as this is not yet supported in SHUTTLE.
             */

            // unique layerID per LayerContainer, does not need to be unique across whole simulation
            var layerId = 0;

            var thereAreGisLayers = scenarioConfig["InitializationDescription"]["GISLayers"].HasValues;

            var distributionPossible = layerContainerClients.Count() > 1;


            var timeSeriesSourceEnumerator = scenarioConfig["InitializationDescription"]["TimeSeriesLayers"]
                .Children()
                .GetEnumerator();
            var thereAreTimeSeriesLayers = timeSeriesSourceEnumerator.MoveNext();

            var obstacleLayerSourceEnumerator = scenarioConfig["InitializationDescription"]["ObstacleLayers"]
                .Children()
                .GetEnumerator();
            var thereAreObstacleLayer = obstacleLayerSourceEnumerator.MoveNext();

            var geoPotentialFieldLayerSourceEnumerator =
                scenarioConfig["InitializationDescription"]["GeoPotentialFieldLayers"].Children().GetEnumerator();
            var thereAreGeoPotentialFieldLayers = geoPotentialFieldLayerSourceEnumerator.MoveNext();

            var gridPotentialFieldLayerSourceEnumerator =
                scenarioConfig["InitializationDescription"]["GridPotentialFieldLayers"].Children().GetEnumerator();
            var thereAreGridPotentialFieldLayers = gridPotentialFieldLayerSourceEnumerator.MoveNext();

            foreach (var layerDescription in _modelContainer.GetInstantiationOrder(modelDescription))
            {
                var layerInstanceId = new TLayerInstanceId(layerDescription, layerId);


                var globalParams = scenarioConfig["ParameterizationDescription"]["Global"];
                var startDate = DateTime.Parse(globalParams["SimulationStartDateTime"].ToString());
                var endDate = DateTime.Parse(globalParams["SimulationEndDateTime"].ToString());
                var deltaT = int.Parse(globalParams["DeltaT"].ToString());
                var deltaTUnit = globalParams["DeltaTUnit"].ToString();

                var simStepDuration = GetDeltaTUnitTimeSpan(deltaTUnit, deltaT);

                var initData = new TInitData(false, simStepDuration, startDate, _simulationId,
                    MARSConfigServiceSettings.Address);

                // fetch layerConfig by layerName
                LayerConfig layerConfig;
                try
                {
                    layerConfig = modelConfig.LayerConfigs.First(cfg => cfg.LayerName == layerDescription.Name);
                }
                catch
                {
                    throw new NoLayerConfigurationPresentException(
                        "Please specify an appropriate LayerConfig for " + layerDescription.Name +
                        " in your config file: " + modelDescription.Name +
                        ".cfg");
                }

                // make distinction between distributed initialization...

                #region distributed init

                if (distributionPossible && layerConfig.DistributionStrategy != DistributionStrategy.NO_DISTRIBUTION)
                {
                    // currently we only support EVEN_DISTRIBUTION or ENV_REPLICATION
                    // each of which lead to the distributed layers being instantiated on all nodes
                    foreach (var lc in layerContainerClients)
                    {
                        lc.Instantiate(layerInstanceId);
                    }

                    // check if the current layer is a GIS Layer
                    var layerType = Type.GetType(layerDescription.AssemblyQualifiedName);
                    var interfaces = layerType.GetInterfaces();

                    // TODO: Maybe reimplement some day with new GIS Library
                    /*if (thereAreGisLayers && interfaces.Contains(typeof(IGISAccess)))
                    {
                        var gisInfo = gisLayerSourceEnumerator.Current;
                        initData.AddGisInitConfig(gisInfo.GISFileName, gisInfo.LayerNames.ToArray());


                        //...and finally initialize all layer instances with it
                        foreach (var lc in layerContainerClients)
                        {
                            lc.Initialize(layerInstanceId, initData);
                        }

                        if (!gisLayerSourceEnumerator.MoveNext())
                        {
                            thereAreGisLayers = false;
                        }

                    }
                    else*/
                    if (thereAreTimeSeriesLayers && interfaces.Contains(typeof(ITimeSeriesLayer)))
                    {
                        var tsInfo = timeSeriesSourceEnumerator.Current;
                        initData.AddTimeSeriesInitConfig(tsInfo["TableName"].ToString(),
                            tsInfo["ColumnName"].ToString(), tsInfo["ColumnClearName"].ToString());

                        foreach (var lc in layerContainerClients)
                        {
                            lc.Initialize(layerInstanceId, initData);
                        }

                        if (!timeSeriesSourceEnumerator.MoveNext())
                        {
                            thereAreTimeSeriesLayers = false;
                        }
                    }
                    else if (thereAreObstacleLayer && interfaces.Contains(typeof(IObstacleLayer)))
                    {
                        var olInfo = obstacleLayerSourceEnumerator.Current;
                        initData.AddFileInitInfo(olInfo["MetaDataId"].ToString());
                        if (!obstacleLayerSourceEnumerator.MoveNext())
                        {
                            thereAreObstacleLayer = false;
                        }
                    }
                    else if (thereAreGridPotentialFieldLayers && interfaces.Contains(typeof(IGridPotentialFieldLayer)))
                    {
                        var gridInfo = gridPotentialFieldLayerSourceEnumerator.Current;
                        initData.AddFileInitInfo(gridInfo["MetaDataId"].ToString());
                        if (!gridPotentialFieldLayerSourceEnumerator.MoveNext())
                        {
                            thereAreGridPotentialFieldLayers = false;
                        }
                    }
                    else if (thereAreGeoPotentialFieldLayers && interfaces.Contains(typeof(IGeoPotentialFieldLayer)))
                    {
                        var geoInfo = geoPotentialFieldLayerSourceEnumerator.Current;
                        initData.AddFileInitInfo(geoInfo["MetaDataId"].ToString());
                        if (!geoPotentialFieldLayerSourceEnumerator.MoveNext())
                        {
                            thereAreGeoPotentialFieldLayers = false;
                        }
                    }
                    else if (scenarioConfig["InitializationDescription"]["BasicLayers"]
                        .Children()
                        .Any(j => j["LayerName"].ToString() == layerDescription.Name))
                    {
                        var basicLayerMapping = (JObject) scenarioConfig["InitializationDescription"]["BasicLayers"]
                            .Children()
                            .First(j => j["LayerName"].ToString() == layerDescription.Name);

                        initData = new TInitData(false, simStepDuration, startDate, _simulationId,
                            MARSConfigServiceSettings.Address);

                        foreach (var agentMapping in basicLayerMapping["Agents"])
                        {
                            var agentCount = int.Parse(agentMapping["InstanceCount"].ToString());
                            var lcCount = layerContainerClients.Count();
                            var normalAgentCount = agentCount / lcCount;
                            var overheadAgentCount = agentCount % lcCount;

                            Parallel.For(0, lcCount, i =>
                            {
                                // add overhead of agents to first layer
                                var actualAgentCount = i == 0
                                    ? normalAgentCount + overheadAgentCount
                                    : normalAgentCount;
                                var offset = i * actualAgentCount;

                                initData.AddAgentInitConfig(
                                    agentMapping["Name"].ToString(),
                                    agentMapping["FullName"].ToString(),
                                    actualAgentCount,
                                    offset,
                                    new List<TConstructorParameterMapping>(
                                        agentMapping["ConstructorParameterMapping"]
                                            .Children()
                                            .Select(j =>
                                            {
                                                if (j["TableName"] != null)
                                                {
                                                    return new TConstructorParameterMapping(
                                                        j["Type"].ToString(),
                                                        j["Name"].ToString(),
                                                        bool.Parse(j["IsAutoInitialized"].ToString()),
                                                        j["MappingType"].ToString(),
                                                        j["TableName"].ToString(),
                                                        j["ColumnName"].ToString()
                                                    );
                                                }

                                                return new TConstructorParameterMapping(
                                                    j["Type"].ToString(),
                                                    j["Name"].ToString(),
                                                    bool.Parse(j["IsAutoInitialized"].ToString()),
                                                    j["MappingType"].ToString(),
                                                    value: j["Value"].ToString()
                                                );
                                            })
                                            .ToList()
                                    )
                                );
                                layerContainerClients[i].Initialize(layerInstanceId, initData);
                            });
                        }
                    }
                }

                #endregion

                // ... and non-distributed initialization
                else
                {
                    Console.WriteLine($"INIT OF: {layerInstanceId.LayerDescription.Name}");

                    layerContainerClients[0].Instantiate(layerInstanceId);


                    var layerType = Type.GetType(layerDescription.FullName)
                                    ??
                                    new LayerLoader.Implementation.LayerLoader()
                                        .LoadAllLayersForModel(modelDescription.ModelPath)
                                        .FirstOrDefault(
                                            l => l.LayerType.AssemblyQualifiedName.Equals(layerDescription
                                                .AssemblyQualifiedName))
                                        .LayerType;

                    var interfaces = layerType.GetTypeInfo().GetInterfaces();
                    /*
                    if (thereAreGisLayers && interfaces.Contains(typeof(IGISAccess)))
					{
						var gisInfo = gisLayerSourceEnumerator.Current;
						initData.AddGisInitConfig(gisInfo.GISFileName, gisInfo.LayerNames.ToArray());
						if (!gisLayerSourceEnumerator.MoveNext())
						{
							thereAreGisLayers = false;
						}
					}
					else
                    */
                    if (thereAreTimeSeriesLayers && interfaces.Contains(typeof(ITimeSeriesLayer)))
                    {
                        var tsInfo = timeSeriesSourceEnumerator.Current;
                        initData.AddTimeSeriesInitConfig(tsInfo["TableName"].ToString(),
                            tsInfo["ColumnName"].ToString(), tsInfo["ColumnClearName"].ToString());
                        if (!timeSeriesSourceEnumerator.MoveNext())
                        {
                            thereAreTimeSeriesLayers = false;
                        }
                    }
                    else if (thereAreObstacleLayer && interfaces.Contains(typeof(IObstacleLayer)))
                    {
                        var olInfo = obstacleLayerSourceEnumerator.Current;
                        initData.AddFileInitInfo(olInfo["MetaDataId"].ToString());
                        if (!obstacleLayerSourceEnumerator.MoveNext())
                        {
                            thereAreObstacleLayer = false;
                        }
                    }
                    else if (thereAreGridPotentialFieldLayers && interfaces.Contains(typeof(IGridPotentialFieldLayer)))
                    {
                        var gridInfo = gridPotentialFieldLayerSourceEnumerator.Current;
                        initData.AddFileInitInfo(gridInfo["MetaDataId"].ToString());
                        if (!gridPotentialFieldLayerSourceEnumerator.MoveNext())
                        {
                            thereAreGridPotentialFieldLayers = false;
                        }
                    }
                    else if (thereAreGeoPotentialFieldLayers && interfaces.Contains(typeof(IGeoPotentialFieldLayer)))
                    {
                        var geoInfo = geoPotentialFieldLayerSourceEnumerator.Current;
                        initData.AddFileInitInfo(geoInfo["MetaDataId"].ToString());
                        if (!geoPotentialFieldLayerSourceEnumerator.MoveNext())
                        {
                            thereAreGeoPotentialFieldLayers = false;
                        }
                    }
                    else if (scenarioConfig["InitializationDescription"]["BasicLayers"]
                        .Children()
                        .Any(j => j["LayerName"].ToString() == layerDescription.Name))
                    {
                        var basicLayerMapping = scenarioConfig["InitializationDescription"]["BasicLayers"]
                            .Children()
                            .First(j => j["LayerName"].ToString() == layerDescription.Name);

                        initData = new TInitData(false, simStepDuration, startDate, _simulationId,
                            MARSConfigServiceSettings.Address);

                        foreach (var agentMapping in basicLayerMapping["Agents"])
                        {
                            initData.AddAgentInitConfig(
                                agentMapping["Name"].ToString(),
                                agentMapping["FullName"].ToString(),
                                int.Parse(agentMapping["InstanceCount"].ToString()),
                                0,
                                new List<TConstructorParameterMapping>(
                                    agentMapping["ConstructorParameterMapping"]
                                        .Children()
                                        .Select(j =>
                                        {
                                            if (j["TableName"] != null)
                                            {
                                                return new TConstructorParameterMapping(
                                                    j["Type"].ToString(),
                                                    j["Name"].ToString(),
                                                    bool.Parse(j["IsAutoInitialized"].ToString()),
                                                    j["MappingType"].ToString(),
                                                    j["TableName"].ToString(),
                                                    j["ColumnName"].ToString()
                                                );
                                            }
                                            else
                                            {
                                                return new TConstructorParameterMapping(
                                                    j["Type"].ToString(),
                                                    j["Name"].ToString(),
                                                    bool.Parse(j["IsAutoInitialized"].ToString()),
                                                    j["MappingType"].ToString(),
                                                    value: j["Value"].ToString()
                                                );
                                            }
                                        })
                                        .ToList()
                                )
                            );
                        }
                    }

                    layerContainerClients[0].Initialize(layerInstanceId, initData);
                }


                layerId++;
            }

            return layerContainerClients;
        }


        private TimeSpan GetDeltaTUnitTimeSpan(string deltaTUnit, int deltaT)
        {
            TimeSpan simStepDuration;
            switch (deltaTUnit)
            {
                case "years":
                    simStepDuration = new TimeSpan(deltaT * 365, 0, 0, 0);
                    break;
                case "months":
                    simStepDuration = new TimeSpan(deltaT * 30, 0, 0, 0);
                    break;
                case "days":
                    simStepDuration = new TimeSpan(deltaT, 0, 0, 0);
                    break;
                case "hours":
                    simStepDuration = new TimeSpan(0, deltaT, 0, 0);
                    break;
                case "minutes":
                    simStepDuration = new TimeSpan(0, 0, deltaT, 0);
                    break;
                case "seconds":
                    simStepDuration = new TimeSpan(0, 0, 0, deltaT);
                    break;
                case "milliseconds":
                    simStepDuration = new TimeSpan(0, 0, 0, 0, deltaT);
                    break;
                case "microseconds":
                    simStepDuration = new TimeSpan(deltaT * 10);
                    break;
            }
            return simStepDuration;
        }

        /// <summary>
        /// Handler to find new idle Layercontainers
        /// </summary>
        /// <param name="newnode"></param>
        private void NewNode(TNodeInformation newnode)
        {
            lock (this)
            {
                _idleLayerContainers.Add(newnode);
            }
        }
    }
}