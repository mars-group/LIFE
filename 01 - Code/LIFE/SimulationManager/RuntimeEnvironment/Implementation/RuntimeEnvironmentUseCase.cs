using System;
using System.Collections.Generic;
using System.Linq;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using Hik.Communication.ScsServices.Client;
using LCConnector;
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

        public void StartWithModel(TModelDescription model,
                ICollection<TNodeInformation> layerContainerNodes,
                int? nrOfTicks = null) {
            lock (this) {
				if(layerContainerNodes.Count <= 0 || _idleLayerContainers.Count <= 0){
					throw new NoLayerContainersArePresentException ();
				}
                // if not all LayerContainers are idle throw exception
				if (!layerContainerNodes.All (l => _idleLayerContainers.Any (c => c.Equals (l)))) {
					throw new LayerContainerBusyException ();
				}

                // throw Exception if model is already running in this cluster
                // TODO: Is that really intended?
                //if (_steppedSimulations.ContainsKey(model)) throw new SimulationAlreadyRunningException();

                // download Model ZIP file from MARS WebSuite, extract and add it to the model repo
                if (model.SourceURL != String.Empty) {
                    _modelContainer.AddModelFromURL(model.SourceURL);
                }

                IList<LayerContainerClient> clients = InitConnections(model, layerContainerNodes);

                _steppedSimulations[model] = new SteppedSimulationExecutionUseCase(nrOfTicks, clients);
            }
        }

        public void Pause(TModelDescription model) {
            if (!_steppedSimulations.ContainsKey(model)) return;

            _steppedSimulations[model].PauseSimulation();
        }

        public void Resume(TModelDescription model) {
            if (!_steppedSimulations.ContainsKey(model)) return;

            _steppedSimulations[model].ResumeSimulation();
        }

        public void Abort(TModelDescription model) {
            if (!_steppedSimulations.ContainsKey(model)) return;

            _steppedSimulations[model].Abort();
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
            var layerContainerClients = new LayerContainerClient[layerContainers.Count];

            var i = 0;
            foreach (TNodeInformation nodeInformationType in layerContainers) {
                var client = new LayerContainerClient
                    (
                    ScsServiceClientBuilder.CreateClient<ILayerContainer>
                        (
                            nodeInformationType.NodeEndpoint.IpAddress + ":" +
                            nodeInformationType.NodeEndpoint.Port),
                    content,
                    _modelContainer.GetInstantiationOrder(modelDescription),
                    i);
                layerContainerClients[i] = client;
                i++;
            }

            return layerContainerClients;
        }

        private void NewNode(TNodeInformation newnode) {
            lock (this) {
                _idleLayerContainers.Add(newnode);
            }
        }
    }
}