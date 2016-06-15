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
using System.Threading;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using Hik.Communication.ScsServices.Client;
using LCConnector;
using LCConnector.TransportTypes;
using LifeAPI.Config;
using LifeAPI.Layer.GIS;
using LifeAPI.Layer.TimeSeries;
using MARS.Shuttle.SimulationConfig;
using ModelContainer.Interfaces;
using NodeRegistry.Interface;
using RuntimeEnvironment.Implementation.Entities;
using RuntimeEnvironment.Interfaces;
using SMConnector;
using SMConnector.Exceptions;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Implementation
{
	internal class RuntimeEnvironmentUseCase : IRuntimeEnvironment {
        private readonly IModelContainer _modelContainer;
        private readonly INodeRegistry _nodeRegistry;
        private readonly IDictionary<TModelDescription, SteppedSimulationExecutionUseCase> _steppedSimulations;
        private readonly ISet<TNodeInformation> _idleLayerContainers;
        private readonly ISet<TNodeInformation> _busyLayerContainers;
        private Guid _simulationId;

        public RuntimeEnvironmentUseCase
            (
            IModelContainer modelContainer,
            INodeRegistry nodeRegistry) {
            _modelContainer = modelContainer;
            _nodeRegistry = nodeRegistry;

            _steppedSimulations = new Dictionary<TModelDescription, SteppedSimulationExecutionUseCase>();
            _idleLayerContainers = new HashSet<TNodeInformation>();
            _busyLayerContainers = new HashSet<TNodeInformation>();

            _nodeRegistry.SubscribeForNewNodeConnectedByType(NewNode, NodeType.LayerContainer);
        }

        #region IRuntimeEnvironment Members

        public void StartWithModel(Guid simulationId,TModelDescription model, ICollection<TNodeInformation> layerContainerNodes, int? nrOfTicks = null, string simConfigName = "SimConfig.json", bool startPaused = false) {
            _simulationId = simulationId;

			if(layerContainerNodes.Count <= 0 || _idleLayerContainers.Count <= 0){
				throw new NoLayerContainersArePresentException ();
			}

            Console.WriteLine("Found and working with " + layerContainerNodes.Count + " Layercontainers.");

            // if not all LayerContainers are idle throw exception
			if (!layerContainerNodes.All (l => _idleLayerContainers.Any (c => c.Equals (l)))) {
				throw new LayerContainerBusyException ();
			}

            Console.WriteLine("Setting up SimulationRun...");
            var sw = Stopwatch.StartNew();

            IList<LayerContainerClient> clients = SetupSimulationRun(model, layerContainerNodes, simConfigName);

			// try to get SimConfig and determine the number of ticks from it
			var shuttleSimConfig = _modelContainer.GetShuttleSimConfig(model, simConfigName);
			if (shuttleSimConfig != null) {
				var tickCount = shuttleSimConfig.GetSimDurationInSteps();
				if (tickCount > 0) {
					nrOfTicks = tickCount;
				}
			}

            sw.Stop();
            Console.WriteLine("...done in " + sw.ElapsedMilliseconds + "ms or " + sw.Elapsed);

			_steppedSimulations[model] = new SteppedSimulationExecutionUseCase(nrOfTicks, clients, simulationId, startPaused);

        }

        public void StepSimulation(TModelDescription model, ICollection<TNodeInformation> layerContainerNodes, int? nrOfTicks = null) {
            if (!_steppedSimulations.ContainsKey(model))
            {
                throw new SimulationHasNotBeenStartedException
                    ("It appears that you did not start your simulation yet. Please call StartSimulationWithModel(...) first.");
            }  
    	    _steppedSimulations[model].StepSimulation(nrOfTicks);

        }

        public void Pause(TModelDescription model) {
            if (!_steppedSimulations.ContainsKey(model))
            {
                throw new SimulationHasNotBeenStartedException
                    ("It appears that you did not start your simulation yet. Please call StartSimulationWithModel(...) first.");
            }

            _steppedSimulations[model].PauseSimulation();
        }

        public void Resume(TModelDescription model) {
            if (!_steppedSimulations.ContainsKey(model)) {
                throw new SimulationHasNotBeenStartedException
                    ("It appears that you did not start your simulation yet. Please call StartSimulationWithModel(...) first.");
            }

            _steppedSimulations[model].ResumeSimulation();
        }

        public void Abort(TModelDescription model) {
            if (!_steppedSimulations.ContainsKey(model)) return;

            _steppedSimulations[model].Abort();
        }
			

		public void WaitForSimulationToFinish (TModelDescription model)
		{
			if (!_steppedSimulations.ContainsKey(model))
			{
				throw new SimulationHasNotBeenStartedException
				("It appears that you did not start your simulation yet. Please call StartSimulationWithModel(...) first.");
			}
			_steppedSimulations [model].WaitForSimulationToFinish ();
		}

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {}

        #endregion

        /// <summary>
        ///     Establishes connections to the given nodes and instantiates the model on all layer containers, according to the
        ///     instantiation order.
        /// </summary>
        /// <param name="modelDescription">not null</param>
        /// <param name="layerContainers">not null</param>
        /// <returns></returns>
        private LayerContainerClient[] SetupSimulationRun(TModelDescription modelDescription, ICollection<TNodeInformation> layerContainers, string simConfigName) {

            var content = _modelContainer.GetModel(modelDescription);
            var layerContainerClients = new List<LayerContainerClient>();

            /* 1.
             * Create LayerContainerClients for all connected LayerContainers
             */
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
                        content);
                        layerContainerClients.Add(client);
                        connected = true;
                    }
                    catch (Exception ex)
                    {
                        // fail after 3 attempts
                        if (retries >= 3)
                        {
                            var sockEx = ex as SocketException;
                            if (sockEx != null)
                            {
                                Console.Error.WriteLine(
                                    "A LayerContainer could not be connected. Continueing without it. Exception was: {0}",
                                    sockEx.Message);
                            }
                            else
                            {
                                throw;
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

            /* Load configuration and determine which one to use. SHUTTLE files will be prefered, old-school
             * XML config files are still valid but will be deprecated in the near future
             */
            var modelConfig = _modelContainer.GetModelConfig(modelDescription);
            var shuttleSimConfig = _modelContainer.GetShuttleSimConfig(modelDescription, simConfigName);


			// only accept SHUTTLE based configuration
			if (shuttleSimConfig != null)
			{
				// configure bia SHUTTLE
				return SetupSimulationRunViaShuttleConfig(modelDescription, layerContainerClients.ToArray(), shuttleSimConfig, modelConfig);
			}
			throw new Exception("No SHUTTLE SimConfig has been found. Please check your root folder for a SimConfig file.");
		}


        private LayerContainerClient[] SetupSimulationRunViaShuttleConfig(TModelDescription modelDescription, LayerContainerClient[] layerContainerClients, ISimConfig shuttleSimConfig, ModelConfig modelConfig)
        {
            /* 2.
             * Instantiate and initialize Layers by InstantiationOrder,
             * For now don'tdifferentiate between distributable and non-distributable layers
             * as this is not yet supported in SHUTTLE. 
             */

            // unique layerID per LayerContainer, does not need to be unique across whole simulation 
            var layerId = 0;
            var gisLayerSourceEnumerator = shuttleSimConfig.GetGISActiveLayerSources().GetEnumerator();
            var thereAreGisLayers = gisLayerSourceEnumerator.MoveNext();

			var distributionPossible = layerContainerClients.Count() > 1;

            var timeSeriesSourceEnumerator = shuttleSimConfig.GetTSLayerSources().GetEnumerator();
            var thereAreTimeSeriesLayers = timeSeriesSourceEnumerator.MoveNext();

            foreach (var layerDescription in _modelContainer.GetInstantiationOrder(modelDescription))
            {
                var layerInstanceId = new TLayerInstanceId(layerDescription, layerId);

				var initData = new TInitData(false, shuttleSimConfig.GetSimStepDuration(), shuttleSimConfig.GetSimStartDate(), _simulationId);

				// fetch layerConfig by layerName
				LayerConfig layerConfig;
				try
				{
					layerConfig = modelConfig.LayerConfigs.First(cfg => cfg.LayerName == layerDescription.Name);
				}
				catch
				{
					throw new NoLayerConfigurationPresentException(
						"Please specify an appropriate LayerConfig for " + layerDescription.Name + " in your config file: " + modelDescription.Name +
						".cfg");
				}

				// make distinction between distributed initialization...
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
					if (thereAreGisLayers && interfaces.Contains(typeof(IGISAccess)))
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
					else if (thereAreTimeSeriesLayers && interfaces.Contains(typeof(ITimeSeriesLayer)))
					{
						var tsInfo = timeSeriesSourceEnumerator.Current;
						initData.AddTimeSeriesInitConfig(tsInfo.TableName, tsInfo.ColumnName, tsInfo.ClearColumnName);

						foreach (var lc in layerContainerClients)
						{
							lc.Initialize(layerInstanceId, initData);
						}

						if (!timeSeriesSourceEnumerator.MoveNext())
						{
							thereAreTimeSeriesLayers = false;
						}
					}
					else if (shuttleSimConfig.GetIAtLayerInfo()
						  .GetAtConstructorInfoListsWithLayerName()
						  .ContainsKey(layerDescription.Name))
					{


						foreach (var agentConfig in shuttleSimConfig.GetIAtLayerInfo().GetAtConstructorInfoListsWithLayerName()[layerDescription.Name])
						{
							var agentCount = agentConfig.GetAgentInstanceCount();
							var lcCount = layerContainerClients.Count();
							var normalAgentCount = agentCount / lcCount;
							var overheadAgentCount = agentCount % lcCount;

							Parallel.For(0, lcCount, i => {

								initData = new TInitData(false, shuttleSimConfig.GetSimStepDuration(), shuttleSimConfig.GetSimStartDate(), _simulationId);

								// add overhead of agents to first layer
								var actualAgentCount = i == 0 ? normalAgentCount + overheadAgentCount : normalAgentCount;
								var offset = i * actualAgentCount;

								initData.AddAgentInitConfig(
									agentConfig.GetClassName(),
									agentConfig.GetFullName(),
									actualAgentCount,
									offset,
									agentConfig.GetFieldToConstructorArgumentRelations()
								);
								layerContainerClients[i].Initialize(layerInstanceId, initData);
							});
						}

					}
				}
				// ... and non-distributed initialization
				else 
				{
					layerContainerClients[0].Instantiate(layerInstanceId);

					// check if the current layer is a GIS Layer
					var layerType = Type.GetType(layerDescription.AssemblyQualifiedName);
					var interfaces = layerType.GetInterfaces();
					if (thereAreGisLayers && interfaces.Contains(typeof(IGISAccess)))
					{
						var gisInfo = gisLayerSourceEnumerator.Current;
						initData.AddGisInitConfig(gisInfo.GISFileName, gisInfo.LayerNames.ToArray());
						if (!gisLayerSourceEnumerator.MoveNext())
						{
							thereAreGisLayers = false;
						}
					}
					else if (thereAreTimeSeriesLayers && interfaces.Contains(typeof(ITimeSeriesLayer)))
					{
						var tsInfo = timeSeriesSourceEnumerator.Current;
						initData.AddTimeSeriesInitConfig(tsInfo.TableName, tsInfo.ColumnName, tsInfo.ClearColumnName);
						if (!timeSeriesSourceEnumerator.MoveNext())
						{
							thereAreTimeSeriesLayers = false;
						}
					}
					else if (shuttleSimConfig.GetIAtLayerInfo()
						  .GetAtConstructorInfoListsWithLayerName()
						  .ContainsKey(layerDescription.Name))
					{

						foreach (var agentConfig in shuttleSimConfig.GetIAtLayerInfo().GetAtConstructorInfoListsWithLayerName()[layerDescription.Name])
						{
							var agentCount = agentConfig.GetAgentInstanceCount();

							initData.AddAgentInitConfig(
								agentConfig.GetClassName(),
								agentConfig.GetFullName(),
								agentCount,
								0,
								agentConfig.GetFieldToConstructorArgumentRelations()
								);
						}

					}

					layerContainerClients[0].Initialize(layerInstanceId, initData);
				}
                

                layerId++;
            }

            return layerContainerClients;
        }



        private void NewNode(TNodeInformation newnode) {
            lock (this) {
                _idleLayerContainers.Add(newnode);
                Console.WriteLine("New LayerContainer registered: IP={0}, Name={1}", newnode.NodeEndpoint.IpAddress, newnode.NodeIdentifier);
            }
        }
    }
}