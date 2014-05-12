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
            _runtimeEnvironmentUseCase = new RuntimeEnvironmentUseCase(modelContainer, layerRegistry);
        }

        public void StartWithModel(TModelDescription model, ICollection<TNodeInformation> layerContainerNodes,
            int? nrOfTicks = null) {
            _runtimeEnvironmentUseCase.StartWithModel(model, layerContainerNodes, nrOfTicks);
        }

        public void Pause(TModelDescription model) {
            _runtimeEnvironmentUseCase.Pause(model);
        }

        public void Resume(TModelDescription model) {
            _runtimeEnvironmentUseCase.Resume(model);
        }

        public void Abort(TModelDescription model) {
            _runtimeEnvironmentUseCase.Abort(model);
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            _runtimeEnvironmentUseCase.SubscribeForStatusUpdate(statusUpdateAvailable);
        }
    }
}