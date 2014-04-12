using System;
using System.Collections.Generic;
using System.Linq;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using LCConnector.TransportTypes;
using ModelContainer.Interfaces;
using NodeRegistry.Interface;
using RuntimeEnvironment.Interfaces;
using Shared;
using SMConnector;
using SMConnector.Exceptions;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Implementation {
    internal class RuntimeEnvironmentUseCase : IRuntimeEnvironment {
        private readonly SimulationManagerSettings _settings;
        private readonly IModelContainer _modelContainer;
        private readonly INodeRegistry _nodeRegistry;
        private readonly IDictionary<TModelDescription, SteppedSimulationExecutionUseCase> _steppedSimulations;
        private readonly ISet<NodeInformationType> _idleLayerContainers;
        private readonly ISet<NodeInformationType> _busyLayerContainers;

        public RuntimeEnvironmentUseCase(SimulationManagerSettings settings,
            IModelContainer modelContainer,
            INodeRegistry nodeRegistry) {
            _settings = settings;
            _modelContainer = modelContainer;
            _nodeRegistry = nodeRegistry;
            _steppedSimulations = new Dictionary<TModelDescription, SteppedSimulationExecutionUseCase>();
            _idleLayerContainers = new HashSet<NodeInformationType>();
            _busyLayerContainers = new HashSet<NodeInformationType>();
        }

        public void StartSimulationWithModel(TModelDescription model, ICollection<NodeInformationType> layerContainers,
            int? nrOfTicks = null) {
            lock (this) {
                if (!layerContainers.All(l => _idleLayerContainers.Any(c => c.Equals(l)))) {
                    throw new LayerContainerBusyException();
                }

                if (_steppedSimulations.ContainsKey(model)) {
                    throw new SimulationAlreadyRunningException();
                }
            }

            IList<TLayerDescription> instantiationOrder = _modelContainer.GetInstantiationOrder(model);
            List<NodeInformationType> lcNodes = _nodeRegistry.GetAllNodesByType(NodeType.LayerContainer);

            foreach (var layerDescription in instantiationOrder) {
                
            }
        }

        public void StartSimulationWithModel(TModelDescription model, ICollection<int> layerContainers, int? nrOfTicks = null) {
            throw new NotImplementedException();
        }

        public void PauseSimulation(TModelDescription model) {
            throw new NotImplementedException();
        }

        public void ResumeSimulation(TModelDescription model) {
            throw new NotImplementedException();
        }

        public void AbortSimulation(TModelDescription model) {
            throw new NotImplementedException();
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            throw new NotImplementedException();
        }
    }
}