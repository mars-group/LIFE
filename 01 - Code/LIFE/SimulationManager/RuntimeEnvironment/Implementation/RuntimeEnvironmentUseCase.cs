using ConfigurationAdapter.Interface;
using LayerRegistry.Interfaces;
using ModelContainer.Interfaces;
using RuntimeEnvironment.Interfaces;
using Shared;

namespace RuntimeEnvironment.Implementation
{
    class RuntimeEnvironmentUseCase : IRuntimeEnvironment
    {
        private readonly Configuration<SimulationManagerSettings> _settings;
        private readonly IModelContainer _modelContainer;
        private readonly ILayerRegistry _layerRegistry;

        public RuntimeEnvironmentUseCase(Configuration<SimulationManagerSettings> settings,
                                        IModelContainer modelContainer,
                                        ILayerRegistry layerRegistry) {
            _settings = settings;
            _modelContainer = modelContainer;
            _layerRegistry = layerRegistry;
        }
    }
}
