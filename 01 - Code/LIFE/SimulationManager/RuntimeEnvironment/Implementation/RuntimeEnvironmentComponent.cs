using System.Collections.Generic;
using CommonTypes.DataTypes;
using ModelContainer.Interfaces;
using NodeRegistry.Interface;
using RuntimeEnvironment.Interfaces;
using Shared;
using SMConnector;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Implementation {
    public class RuntimeEnvironmentComponent : IRuntimeEnvironment {
        private readonly IRuntimeEnvironment _runtimeEnvironmentUseCase;

        public RuntimeEnvironmentComponent(SimulationManagerSettings settings,
            IModelContainer modelContainer,
            INodeRegistry layerRegistry) {
            _runtimeEnvironmentUseCase = new RuntimeEnvironmentUseCase(settings, modelContainer, layerRegistry);
        }

        public void StartSimulationWithModel(TModelDescription model, ICollection<NodeInformationType> layerContainers,
            int? nrOfTicks = null) {
            _runtimeEnvironmentUseCase.StartSimulationWithModel(model, layerContainers, nrOfTicks);
        }

        public void PauseSimulation(TModelDescription model) {
            _runtimeEnvironmentUseCase.PauseSimulation(model);
        }

        public void ResumeSimulation(TModelDescription model) {
            _runtimeEnvironmentUseCase.ResumeSimulation(model);
        }

        public void AbortSimulation(TModelDescription model) {
            _runtimeEnvironmentUseCase.AbortSimulation(model);
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            _runtimeEnvironmentUseCase.SubscribeForStatusUpdate(statusUpdateAvailable);
        }
    }
}