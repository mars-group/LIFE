using System;

namespace PartitionManager.Interfaces {
    public interface IPartitionManager {
        bool AddLayer(Uri layerUri, Guid layerID);
    }
}