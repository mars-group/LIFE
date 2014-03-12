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


        public abstract bool InitLayer(LayerInitData layerInitData);

        public Guid GetID()
        {
            return this._id;
        }
        public abstract void AdvanceOneTick();
        public abstract long GetCurrentTick();
    }
}
