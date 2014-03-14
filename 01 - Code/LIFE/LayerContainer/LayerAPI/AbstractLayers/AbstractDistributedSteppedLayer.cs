using System;
using LayerAPI.DataTypes;
using LayerAPI.Interfaces;

namespace LayerAPI.AbstractLayers
{
    public abstract class AbstractDistributedSteppedLayer : ISteppedLayer
    {        
        
        private readonly Guid _id;

        public AbstractDistributedSteppedLayer()
        {
            _id = Guid.NewGuid();
        }


        public abstract bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle);

        public Guid GetID()
        {
            return this._id;
        }

        public abstract long GetCurrentTick();
    }
}
