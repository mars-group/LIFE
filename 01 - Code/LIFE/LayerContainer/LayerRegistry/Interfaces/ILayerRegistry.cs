
namespace LayerRegistry.Interfaces
{
    using System;

    using LayerAPI.Interfaces;

    public interface ILayerRegistry
    {
        ILayer loadLayer(Uri layerUri, Guid layerID);
    }
}
