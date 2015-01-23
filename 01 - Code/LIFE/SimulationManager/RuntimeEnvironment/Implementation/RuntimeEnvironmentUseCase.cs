﻿using System;
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

                Console.WriteLine("Found and working with " + layerContainerNodes.Count + " Layercontainers.");

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
                            nodeInformationType.NodeEndpoint.Port
                        ),
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

                if (layerConfig.DistributionStrategy == DistributionStrategy.NO_DISTRIBUTION)
                {
                    // easy: first instantiate the layer...
                    layerContainerClients[0].Instantiate(layerInstanceId);

                    //...fetch all agentTypes and amounts...
                    var initData = new TInitData(false, modelConfig.OneTickTimeSpan);
                    foreach (var agentConfig in layerConfig.AgentConfigs)
                    {
                        var ids = new Guid[agentConfig.AgentCount];
                        for (int j = 0; j < agentConfig.AgentCount; j++)
                        {
                            ids[0] = Guid.NewGuid();
                        }
                        initData.AddAgentInitConfig(agentConfig.AgentName, agentConfig.AgentCount, 0, ids, new Guid[0]);
                    }
                    //...and finally initialize the layer with it
                    layerContainerClients[0].Initialize(layerInstanceId, initData);
                }
                else {
                    // get initData by layerConfig and LayerContainers
                    var initData = GetInitDataByLayerConfig(layerConfig, layerContainerClients, modelConfig);

                    foreach (var layerContainerClient in layerContainerClients)
                    {
                        layerContainerClient.Instantiate(layerInstanceId);
                        layerContainerClient.Initialize(layerInstanceId, initData[layerContainerClient]);
                    }
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
        private IDictionary<LayerContainerClient, TInitData> GetInitDataByLayerConfig(LayerConfig layerConfig, LayerContainerClient[] layerContainerClients, ModelConfig modelConfig)
        {
            if (layerContainerClients == null) throw new ArgumentNullException("layerContainerClients");
            
            var result = new Dictionary<LayerContainerClient, TInitData>();

            switch (layerConfig.DistributionStrategy)
            {

                    case DistributionStrategy.EVEN_DISTRIBUTION:

                    // initialize result Dictionary
                    foreach (var layerContainerClient in layerContainerClients)
                    {
                        result.Add(layerContainerClient, new TInitData(true, modelConfig.OneTickTimeSpan));
                    }

                    var lcCount = layerContainerClients.Length;
                    
                    foreach (var agentConfig in layerConfig.AgentConfigs)
                    {
                        
                        // create Guids for all agents
                        var agentIds = new Guid[agentConfig.AgentCount];
                        for (int i = 0; i < agentConfig.AgentCount; i++) {
                            agentIds[i] = Guid.NewGuid();
                        }

                        // calculate Agents per LayerContainer
                        var agentAmountPerLayerContainer = agentConfig.AgentCount / lcCount;
                        
                        // calculate overhead resulting from uneven division
                        var agentOverhead = agentConfig.AgentCount % lcCount;
                        var overheadedAmount = agentAmountPerLayerContainer + agentOverhead;

                        var agentInitIndex = 0;
                        for (var lcIndex = 0; lcIndex < lcCount; lcIndex++) {
                            // add overhead to first layerContainer
                            if (lcIndex == 0)
                            {

                                var shadowAgentCountPerLayerContainer = agentConfig.AgentCount - overheadedAmount;

                                var realAgentIds = new Guid[overheadedAmount];
                                var shadowAgentIds = new Guid[shadowAgentCountPerLayerContainer];

                                for (int j = 0; j < overheadedAmount; j++) {
                                    realAgentIds[j] = agentIds[lcIndex + j];
                                }

                                agentInitIndex += overheadedAmount;

                                for (int j = 0; j < shadowAgentCountPerLayerContainer; j++) {
                                    shadowAgentIds[j] = agentIds[overheadedAmount + j];
                                }

                                result[layerContainerClients[lcIndex]]
                                    .AddAgentInitConfig(
                                        agentConfig.AgentName,
                                        overheadedAmount,
                                        shadowAgentCountPerLayerContainer,
                                        realAgentIds,
                                        shadowAgentIds
                                    );
                            }
                            else
                            {

                                var shadowAgentCountPerLayerContainer = agentConfig.AgentCount-agentAmountPerLayerContainer;

                                var realAgentIds = new Guid[agentAmountPerLayerContainer];
                                var shadowAgentIds = new Guid[shadowAgentCountPerLayerContainer];

                                for (int j = 0; j < agentAmountPerLayerContainer; j++)
                                {
                                    realAgentIds[j] = agentIds[agentInitIndex + j];
                                }
                                // set initIndex to next block of agents
                                agentInitIndex += agentAmountPerLayerContainer;

                                // add all agentIds which are before the current range of real agents
                                for (int j = 0; j < agentInitIndex-agentAmountPerLayerContainer; j++)
                                {
                                    shadowAgentIds[j] = agentIds[j];
                                }

                                // add all agentIds which are after the current range of real agents
                                for (int i = agentInitIndex-agentAmountPerLayerContainer; i < agentConfig.AgentCount-agentInitIndex; i++) {
                                    shadowAgentIds[i] = agentIds[i + agentAmountPerLayerContainer];
                                }

                                result[layerContainerClients[lcIndex]]
                                    .AddAgentInitConfig(
                                        agentConfig.AgentName,
                                        agentAmountPerLayerContainer,
                                        shadowAgentCountPerLayerContainer,
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