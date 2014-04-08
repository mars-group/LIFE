using System;
using DistributedKeyValueStore.Interface;
using LayerAPI.Interfaces;
using LayerRegistry.Interfaces;
using NodeRegistry.Interface;

namespace LayerRegistry.Implementation {
    public class LayerRegistryComponent : ILayerRegistry {
        private readonly ILayerRegistry _layerRegistryUseCase;

        public LayerRegistryComponent(IDistributedKeyValueStore distributedKeyValueStore) {
            _layerRegistryUseCase = new LayerRegistryUseCase(distributedKeyValueStore);
        }

        public ILayer RemoveLayerInstance(Type layerType) {
            return _layerRegistryUseCase.RemoveLayerInstance(layerType);
        }

        public void ResetLayerRegistry() {
            _layerRegistryUseCase.ResetLayerRegistry();
        }

        public ILayer GetRemoteLayerInstance(Type layerType) {
            return _layerRegistryUseCase.GetRemoteLayerInstance(layerType);
        }

        public void RegisterLayer(ILayer layer) {
            _layerRegistryUseCase.RegisterLayer(layer);
        }
    }
}