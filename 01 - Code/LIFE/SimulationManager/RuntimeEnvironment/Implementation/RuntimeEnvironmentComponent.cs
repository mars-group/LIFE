using System.Collections.Generic;

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
                                            INodeRegistry layerRegistry)
        {
            _runtimeEnvironmentUseCase = new RuntimeEnvironmentUseCase(settings, modelContainer, layerRegistry);
        }

        public void StartSimulationWithModel(TModelDescription model, ICollection<int> layerContainers, int? nrOfTicks = null) {
            this._runtimeEnvironmentUseCase.StartSimulationWithModel(model, layerContainers, nrOfTicks);
        }

        public void PauseSimulation(TModelDescription model) {
            this._runtimeEnvironmentUseCase.PauseSimulation(model);
        }

        public void ResumeSimulation(TModelDescription model) {
            this._runtimeEnvironmentUseCase.ResumeSimulation(model);
        }

        public void AbortSimulation(TModelDescription model) {
            this._runtimeEnvironmentUseCase.AbortSimulation(model);
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            this._runtimeEnvironmentUseCase.SubscribeForStatusUpdate(statusUpdateAvailable);
        }
    }
}