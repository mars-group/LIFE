using System;
using LayerAPI.DataTypes;
using LayerAPI.Interfaces;


namespace LayerAPI.AbstractLayers
{

    public abstract class AbstractDistributedEventDrivenLayer : IEventDrivenLayer
    {
        private readonly Guid _id;

        public AbstractDistributedEventDrivenLayer()
        {
            _id = Guid.NewGuid();
        }


        public abstract bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle);

        public Guid GetID()
        {
            return this._id;
        }

        public abstract void StartLayer(long pseudoTickDuration = 0);
        public abstract void PauseLayer();
        public abstract void StopLayer();
        public abstract LayerStatus GetLayerStatus();
    }
}
