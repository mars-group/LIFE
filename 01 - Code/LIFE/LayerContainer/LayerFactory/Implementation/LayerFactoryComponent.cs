
using LayerAPI.Interfaces;
using LayerFactory.Interface;
using LayerRegistry.Interfaces;
using LCConnector.TransportTypes.ModelStructure;

namespace LayerFactory.Implementation {
    public class LayerFactoryComponent : ILayerFactory {
        private readonly ILayerFactory _layerFactoryUseCase;

        public LayerFactoryComponent(ILayerRegistry layerRegistry) {
            _layerFactoryUseCase = new LayerFactoryUseCase(layerRegistry);
        }

        public ILayer GetLayer(string layerName) {
            return _layerFactoryUseCase.GetLayer(layerName);
        }

        public void LoadModelContent(ModelContent content) {
            _layerFactoryUseCase.LoadModelContent(content);
        }
    }
}