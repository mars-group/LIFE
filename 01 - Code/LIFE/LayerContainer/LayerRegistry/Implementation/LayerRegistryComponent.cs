namespace LayerRegistry.Implementation
{
    using System;
    using System.Collections.Generic;
    using LayerAPI.AddinLoader;
    using LayerAPI.Interfaces;
    using LayerRegistry.Interfaces;

    public class LayerRegistryComponent : ILayerRegistry
    {
        private readonly ILayerRegistry _layerRegistryUseCase;

        public LayerRegistryComponent()
        {
            _layerRegistryUseCase = new LayerRegistryUseCase(new AddinLoader());

        }

        public ILayer LoadLayer(Uri layerUri, Guid layerID)
        {
            return _layerRegistryUseCase.LoadLayer(layerUri, layerID);
        }

        public ILayer GetLayerByID(Guid layerID)
        {
            return _layerRegistryUseCase.GetLayerByID(layerID);
        }

        public IList<ILayer> GetAllLayers()
        {
            return _layerRegistryUseCase.GetAllLayers();
        }

        public ILayer RemoveLayerInstance(Guid layerID)
        {
            return _layerRegistryUseCase.RemoveLayerInstance(layerID);
        }

        public void ResetLayerRegistry()
        {
            _layerRegistryUseCase.ResetLayerRegistry();
        }
    }
}
