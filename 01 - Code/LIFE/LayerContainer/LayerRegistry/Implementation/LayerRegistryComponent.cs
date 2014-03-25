using LayerAPI.Interfaces;

namespace LayerRegistry.Implementation
{
    using System;
    using Interfaces;

    public class LayerRegistryComponent : ILayerRegistry
    {
        private readonly ILayerRegistry _layerRegistryUseCase;

        public LayerRegistryComponent()
        {
            _layerRegistryUseCase = new LayerRegistryUseCase();

        }
        public ILayer RemoveLayerInstance(Guid layerID)
        {
            return _layerRegistryUseCase.RemoveLayerInstance(layerID);
        }

        public void ResetLayerRegistry()
        {
            _layerRegistryUseCase.ResetLayerRegistry();
        }

        public ILayer GetLayerInstance(Type parameterType)
        {
            return _layerRegistryUseCase.GetLayerInstance(parameterType);
        }

        public void RegisterLayer(ILayer layer)
        {
            _layerRegistryUseCase.RegisterLayer(layer);
        }
    }
}
