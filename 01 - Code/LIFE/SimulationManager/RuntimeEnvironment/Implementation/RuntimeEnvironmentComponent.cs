using System.Collections.Generic;
using ConfigurationAdapter.Interface;
using LayerRegistry.Interfaces;
using ModelContainer.Interfaces;
using NodeRegistry.Interface;
using RuntimeEnvironment.Interfaces;
using Shared;
using SMConnector;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Implementation {
    public class RuntimeEnvironmentComponent : IRuntimeEnvironment {

        private readonly RuntimeEnvironmentUseCase _runtimeEnvironmentUseCase;

        public RuntimeEnvironmentComponent(Configuration<SimulationManagerSettings> settings,
                                            IModelContainer modelContainer,
                                            INodeRegistry layerRegistry)
        {
            _runtimeEnvironmentUseCase = new RuntimeEnvironmentUseCase(settings, modelContainer, layerRegistry);
        }

        public void StartSimulationWithModel(TModelDescription model) {
            ((IRuntimeEnvironment) _runtimeEnvironmentUseCase).StartSimulationWithModel(model);
        }

        public void PauseSimulation(TModelDescription model) {
            ((IRuntimeEnvironment) _runtimeEnvironmentUseCase).PauseSimulation(model);
        }

        public void ResumeSimulation(TModelDescription model) {
            ((IRuntimeEnvironment) _runtimeEnvironmentUseCase).ResumeSimulation(model);
        }

        public void AbortSimulation(TModelDescription model) {
            ((IRuntimeEnvironment) _runtimeEnvironmentUseCase).AbortSimulation(model);
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            ((IRuntimeEnvironment) _runtimeEnvironmentUseCase).SubscribeForStatusUpdate(statusUpdateAvailable);
        }
    }
}