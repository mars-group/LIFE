
using LayerAPI.Interfaces;
using LayerFactory.Interface;
using LayerRegistry.Interfaces;

namespace LayerFactory.Implementation {
    public class LayerFactoryComponent : ILayerFactory {
        private readonly LayerFactoryUseCase _layerFactoryUseCase;

        public LayerFactoryComponent(ILayerRegistry layerRegistry) {
            _layerFactoryUseCase = new LayerFactoryUseCase(layerRegistry);
        }

        public ILayer GetLayer(string layerName) {
            return _layerFactoryUseCase.GetLayer(layerName);
        }
    }
}