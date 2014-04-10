using System.Collections.Generic;
using ConfigurationAdapter.Interface;
using LayerRegistry.Interfaces;
using ModelContainer.Interfaces;
using RuntimeEnvironment.Interfaces;
using Shared;
using SMConnector;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Implementation {
    public class RuntimeEnvironmentComponent : IRuntimeEnvironment {

        private readonly RuntimeEnvironmentUseCase _runtimeEnvironmentUseCase;

        public RuntimeEnvironmentComponent(Configuration<SimulationManagerSettings> settings,
                                            IModelContainer modelContainer,
                                            ILayerRegistry layerRegistry)
        {
            _runtimeEnvironmentUseCase = new RuntimeEnvironmentUseCase(settings, modelContainer, layerRegistry);
        }
    }
}