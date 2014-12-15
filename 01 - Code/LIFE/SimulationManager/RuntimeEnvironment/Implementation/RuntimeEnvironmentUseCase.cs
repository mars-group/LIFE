using System;
using System.Collections.Generic;
using System.Linq;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using Hik.Communication.ScsServices.Client;
using LCConnector;
using LCConnector.TransportTypes;
using LifeAPI.Config;
using ModelContainer.Interfaces;
using NodeRegistry.Interface;
using RuntimeEnvironment.Implementation.Entities;
using RuntimeEnvironment.Interfaces;
using SMConnector;
using SMConnector.Exceptions;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Implementation {
    internal class RuntimeEnvironmentUseCase : IRuntimeEnvironment {
        private readonly IModelContainer _modelContainer;
        private readonly INodeRegistry _nodeRegistry;
        private readonly IDictionary<TModelDescription, SteppedSimulationExecutionUseCase> _steppedSimulations;
        private readonly ISet<TNodeInformation> _idleLayerContainers;
        private readonly ISet<TNodeInformation> _busyLayerContainers;
      
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

        public void StartWithModel(TModelDescription model, ICollection<TNodeInformation> layerContainerNodes, int? nrOfTicks = null, bool startPaused = false) {
            lock (this) {
				if(layerContainerNodes.Count <= 0 || _idleLayerContainers.Count <= 0){
					throw new NoLayerContainersArePresentException ();
				}
                // if not all LayerContainers are idle throw exception
				if (!layerContainerNodes.All (l => _idleLayerContainers.Any (c => c.Equals (l)))) {
					throw new LayerContainerBusyException ();
				}

                // download Model ZIP file from MARS WebSuite, extract and add it to the model repo
                if (model.SourceURL != String.Empty) {
                    _modelContainer.AddModelFromURL(model.SourceURL);
                }

                IList<LayerContainerClient> clients = InitConnections(model, layerContainerNodes);

                _steppedSimulations[model] = new SteppedSimulationExecutionUseCase(nrOfTicks, clients, startPaused);
            }
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

        public void StartVisualization(TModelDescription model, int? nrOfTicksToVisualize = null) {
            if (_steppedSimulations.ContainsKey(model)) {
                _steppedSimulations[model].StartVisualization(nrOfTicksToVisualize);
            }
            else {
                throw new SimulationHasNotBeenStartedException
                    ("It appears that you did not start your simulation yet. Please call StartSimulationWithModel(...) first.");
            }
        }

        public void StopVisualization(TModelDescription model) {
            if (_steppedSimulations.ContainsKey(model))
            {
                _steppedSimulations[model].StopVisualization();
            }
            else {
                throw new SimulationHasNotBeenStartedException
                    ("It appears that you did not start your simulation yet. Please call StartSimulationWithModel(...) first.");
            }
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
        private LayerContainerClient[] InitConnections(TModelDescription modelDescription, ICollection<TNodeInformation> layerContainers) {

            var content = _modelContainer.GetModel(modelDescription);
            var modelConfig = _modelContainer.GetModelConfig(modelDescription);
            var layerContainerClients = new LayerContainerClient[layerContainers.Count];

            /* 1.
             * Create LayerContainerClients for all connected LayerContainers
             */
            var i = 0;
            foreach (TNodeInformation nodeInformationType in layerContainers)
            {
                var client = new LayerContainerClient
                    (
                    ScsServiceClientBuilder.CreateClient<ILayerContainer>
                        (
                            nodeInformationType.NodeEndpoint.IpAddress + ":" +
                            nodeInformationType.NodeEndpoint.Port),
                    content,
                    i);
                layerContainerClients[i] = client;
                i++;
            }

            /* 2.
             * Instantiate and initialize Layers by InstantiationOrder,
             * differentiate between distributable and non-distributable layers.
             * If distributable: instantiate and initialize in all LayerContainers according to DistributionStrategy
             * This also includes initialization of the agent shadowing system.
             * This is possible because each shadow agent stub, only joins a multicast group, but mustn't have a 
             * 1-1 connection to its real agent counterpart
             */

            // unique layerID per LayerContainer, does not need to be unique across whole simulation 
            var layerId = 0;
            foreach (var layerDescription in _modelContainer.GetInstantiationOrder(modelDescription)) {
                var layerInstanceId = new TLayerInstanceId(layerDescription, layerId);

                // fetch layerConfig by layerName
                var layerConfig = modelConfig.LayerConfigs.First(cfg => cfg.LayerName == layerDescription.Name);

                if (layerConfig.Distributable)
                {
                    // get initData by layerConfig and LayerContainers
                    var initData = GetInitDataByLayerConfig(layerConfig, layerContainerClients);

                    foreach (var layerContainerClient in layerContainerClients) {
                        layerContainerClient.Instantiate(layerInstanceId);
                        layerContainerClient.Initialize(layerInstanceId, initData[layerContainerClient]);
                    }
                }
                else {
                    // easy: first instantiate the layer...
                    layerContainerClients[0].Instantiate(layerInstanceId);

                    //...fetch all agentTypes and amounts...
                    var initData = new TInitData();
                    foreach (var agentConfig in layerConfig.AgentConfigs) {
                        var ids = new Guid[agentConfig.AgentCount];
                        for (int j = 0; j < agentConfig.AgentCount; j++) {
                            ids[0] = Guid.NewGuid();
                        }
                        initData.AddAgentInitConfig(agentConfig.AgentName, agentConfig.AgentCount, agentConfig.AgentCount, ids, new Guid[0]);
                    }
                    //...and finally initialize the layer with it
                    layerContainerClients[0].Initialize(layerInstanceId, initData);
                }

                layerId++;
            }

            return layerContainerClients;
        }

        /// <summary>
        /// Creates a Dictionary of initialization data per Layercontainer
        /// </summary>
        /// <param name="layerConfig"></param>
        /// <param name="layerContainerClients"></param>
        /// <returns></returns>
        private IDictionary<LayerContainerClient, TInitData> GetInitDataByLayerConfig(LayerConfig layerConfig, LayerContainerClient[] layerContainerClients)
        {
            if (layerContainerClients == null) throw new ArgumentNullException("layerContainerClients");
            
            var result = new Dictionary<LayerContainerClient, TInitData>();

            switch (layerConfig.DistributionStrategy)
            {

                    case DistributionStrategy.EVEN_DISTRIBUTION:

                    // initialize result Dictionary
                    foreach (var layerContainerClient in layerContainerClients)
                    {
                        result.Add(layerContainerClient, new TInitData());
                    }

                    var lcCount = layerContainerClients.Length;
                    
                    foreach (var agentConfig in layerConfig.AgentConfigs)
                    {
                        // calculate Agents per LacerContainer
                        var agentAmount = agentConfig.AgentCount / lcCount;
                        
                        // calculate overhead resulting from uneven division
                        var agentOverhead = agentConfig.AgentCount % lcCount;

                        for (var i = 0; i < lcCount; i++) {
                            // add overhead to first layerContainer
                            if (i == 0)
                            {
                                var overheadedAmount = agentAmount + agentOverhead;
                                var shadowAgentCount = agentConfig.AgentCount - overheadedAmount;

                                var realAgentIds = new Guid[overheadedAmount];
                                var shadowAgentIds = new Guid[shadowAgentCount];

                                for (var ra = 0; ra < overheadedAmount; ra++)
                                {
                                    realAgentIds[ra] = Guid.NewGuid();
                                }

                                for (var sa = 0; sa < shadowAgentCount; sa++)
                                {
                                    realAgentIds[sa] = Guid.NewGuid();
                                }

                                result[layerContainerClients[i]]
                                    .AddAgentInitConfig(
                                        agentConfig.AgentName,
                                        overheadedAmount,
                                        shadowAgentCount,
                                        realAgentIds,
                                        shadowAgentIds
                                    );
                            }
                            else
                            {

                                var shadowAgentCount = agentConfig.AgentCount-agentAmount;

                                var realAgentIds = new Guid[agentAmount];
                                var shadowAgentIds = new Guid[shadowAgentCount];

                                for (var ra = 0; ra < agentAmount; ra++)
                                {
                                    realAgentIds[ra] = Guid.NewGuid();
                                }

                                for (var sa = 0; sa < shadowAgentCount; sa++)
                                {
                                    realAgentIds[sa] = Guid.NewGuid();
                                }
                                result[layerContainerClients[i]]
                                    .AddAgentInitConfig(
                                        agentConfig.AgentName,
                                        agentAmount,
                                        shadowAgentCount,
                                        realAgentIds,
                                        shadowAgentIds
                                    );   
                            }
                        }
                    }

                    break;

            }
            return result;
        }

        private void NewNode(TNodeInformation newnode) {
            lock (this) {
                _idleLayerContainers.Add(newnode);
            }
        }
    }
}