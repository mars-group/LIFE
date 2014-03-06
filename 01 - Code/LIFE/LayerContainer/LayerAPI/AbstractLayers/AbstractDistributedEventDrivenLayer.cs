using System;
using System.Dynamic;
using LayerAPI.Interfaces;
using LIFE.LayerContainer.LayerAPI.DataTypes;

namespace LayerAPI.AbstractLayers
{
    public abstract class AbstractDistributedEventDrivenLayer : IEventDrivenLayer
    {
        private Guid ID = Guid.NewGuid();

        public AbstractDistributedEventDrivenLayer()
        {
            
        }

        public bool InitLayer(LayerInitData layerInitData)
        {
           //  TODO: Write code
            return true;
        }

        public Guid GetID()
        {
            return this.ID;
        }

        public abstract void StartLayer(long startTime = 0, long pseudoTickDuration = 0);
        public abstract void PauseLayer();
        public abstract void StopLayer();
        public abstract LayerStatus GetLayerStatus();
    }
}
