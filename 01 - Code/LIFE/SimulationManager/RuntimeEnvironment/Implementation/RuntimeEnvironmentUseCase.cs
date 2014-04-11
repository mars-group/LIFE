using System.Collections.Generic;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using LCConnector.TransportTypes;
using ModelContainer.Interfaces;
using NodeRegistry.Interface;
using RuntimeEnvironment.Interfaces;
using Shared;
using SMConnector;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Implementation
{
    class RuntimeEnvironmentUseCase : IRuntimeEnvironment
    {
        private readonly SimulationManagerSettings _settings;
        private readonly IModelContainer _modelContainer;
        private readonly INodeRegistry _nodeRegistry;
        private readonly IDictionary<TModelDescription, SteppedSimulationExecutionUseCase> _steppedSimulations;
        private readonly ISet<NodeInformationType> busyNodes;

        public RuntimeEnvironmentUseCase(SimulationManagerSettings settings,
                                        IModelContainer modelContainer,
                                        INodeRegistry nodeRegistry) {
            _settings = settings;
            _modelContainer = modelContainer;
            _nodeRegistry = nodeRegistry;
            _steppedSimulations = new Dictionary<TModelDescription, SteppedSimulationExecutionUseCase>();
        }

        public void StartSimulationWithModel(TModelDescription model) {
            IList<TLayerDescription> instantiationOrder = _modelContainer.GetInstantiationOrder(model);

            List<NodeInformationType> lcNodes =_nodeRegistry.GetAllNodesByType(NodeType.LayerContainer);

            if (!_steppedSimulations.ContainsKey(model)) {
                //_steppedSimulations
            }
            
        }

        public void PauseSimulation(TModelDescription model) {
            throw new System.NotImplementedException();
        }

        public void ResumeSimulation(TModelDescription model) {
            throw new System.NotImplementedException();
        }

        public void AbortSimulation(TModelDescription model) {
            throw new System.NotImplementedException();
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            throw new System.NotImplementedException();
        }
    }
}
