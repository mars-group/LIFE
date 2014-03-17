using System;
using System.Collections.Generic;


namespace LayerRegistry.Implementation
{
    using LayerAPI.AddinLoader;
    using LayerAPI.Interfaces;

    using LayerRegistry.Interfaces;

    class LayerRegistryUseCase : ILayerRegistry
    {
        private readonly IAddinLoader addinLoader;

        public LayerRegistryUseCase(IAddinLoader addinLoader)
        {
            this.addinLoader = addinLoader;
        }

        public ILayer LoadLayer(Uri layerUri, Guid layerID)
        {
            throw new NotImplementedException();
        }

        public ILayer GetLayerByID(Guid layerID)
        {
            throw new NotImplementedException();
        }

        public IList<ILayer> GetAllLayers()
        {
            throw new NotImplementedException();
        }

        public ILayer RemoveLayerInstance(Guid layerID)
        {
            throw new NotImplementedException();
        }

        public void ResetLayerRegistry()
        {
            throw new NotImplementedException();
        }
    }
}
