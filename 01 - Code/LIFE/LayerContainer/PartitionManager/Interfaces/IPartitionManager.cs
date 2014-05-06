using System;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;

namespace PartitionManager.Interfaces {
    public interface IPartitionManager {
        bool AddLayer(TLayerInstanceId instanceId);
        void LoadModelContent(ModelContent content);
    }
}