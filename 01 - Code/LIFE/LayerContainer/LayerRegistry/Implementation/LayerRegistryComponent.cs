using System;
using DistributedKeyValueStore.Interface;
using LayerAPI.Interfaces;
using LayerRegistry.Interfaces;
using LayerRegistry.Interfaces.Config;

namespace LayerRegistry.Implementation {
    public class LayerRegistryComponent : ILayerRegistry {
        private readonly ILayerRegistry _layerRegistryUseCase;

        public LayerRegistryComponent(IDistributedKeyValueStore distributedKeyValueStore, LayerRegistryConfig layerRegistryConfig) {
            _layerRegistryUseCase = new LayerRegistryUseCase(distributedKeyValueStore, layerRegistryConfig);
        }

        public ILayer RemoveLayerInstance(Type layerType) {
            return _layerRegistryUseCase.RemoveLayerInstance(layerType);
        }

        public void ResetLayerRegistry() {
            _layerRegistryUseCase.ResetLayerRegistry();
        }

        public ILayer GetLayerInstance(Type layerType) {
            return _layerRegistryUseCase.GetLayerInstance(layerType);
        }

        public void RegisterLayer(ILayer layer) {
            _layerRegistryUseCase.RegisterLayer(layer);
        }
    }
}