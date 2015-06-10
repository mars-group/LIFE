using System;
using System.Collections.Generic;
using CommonTypes.DataTypes;
using ModelContainer.Interfaces;
using NodeRegistry.Interface;
using RuntimeEnvironment.Interfaces;
using SMConnector;
using SMConnector.TransportTypes;
using SimulationManagerShared;

namespace RuntimeEnvironment.Implementation {
    public class RuntimeEnvironmentComponent : IRuntimeEnvironment {
        private readonly IRuntimeEnvironment _runtimeEnvironmentUseCase;

        public RuntimeEnvironmentComponent(SimulationManagerSettings settings,
            IModelContainer modelContainer,
            INodeRegistry layerRegistry) {
            _runtimeEnvironmentUseCase = new RuntimeEnvironmentUseCase(modelContainer, layerRegistry);
        }

        public void StartWithModel
            (Guid simulationId,TModelDescription model, ICollection<TNodeInformation> layerContainerNodes, int? nrOfTicks = null, bool startPaused = false) {
                _runtimeEnvironmentUseCase.StartWithModel(simulationId, model, layerContainerNodes, nrOfTicks, startPaused);
        }

        public void StepSimulation(TModelDescription model, ICollection<TNodeInformation> layerContainerNodes, int? nrOfTicks = null) {
            _runtimeEnvironmentUseCase.StepSimulation(model, layerContainerNodes, nrOfTicks);
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

        public void StartVisualization(TModelDescription model, int? nrOfTicksToVisualize = null) {
            _runtimeEnvironmentUseCase.StartVisualization(model, nrOfTicksToVisualize);
        }

        public void StopVisualization(TModelDescription model) {
            _runtimeEnvironmentUseCase.StopVisualization(model);
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            _runtimeEnvironmentUseCase.SubscribeForStatusUpdate(statusUpdateAvailable);
        }
    }
}