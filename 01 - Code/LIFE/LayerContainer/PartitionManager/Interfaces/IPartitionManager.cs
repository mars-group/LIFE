namespace PartitionManager.Interfaces
{
    using System;

    public interface IPartitionManager
    {
        bool AddLayer(Uri layerUri, Guid layerID);
    }
}
