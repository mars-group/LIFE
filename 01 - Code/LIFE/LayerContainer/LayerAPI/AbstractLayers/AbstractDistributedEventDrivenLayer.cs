using System;
using System.Reflection;
using LayerAPI.Interfaces;
using LIFE.LayerContainer.LayerAPI.DataTypes;
using Mono.Addins;

namespace LayerAPI.AbstractLayers
{
    [assembly:Addin]
    [assembly:AddinDependency("LayerContainer","0,1")]
    
    [Extension]
    public abstract class AbstractDistributedEventDrivenLayer : IEventDrivenLayer
    {
        private Guid ID;

        public AbstractDistributedEventDrivenLayer()
        {
            ID = Guid.NewGuid();
        }


        public abstract bool InitLayer(LayerInitData layerInitData);

        public Guid GetID()
        {
            return this.ID;
        }

        public abstract void StartLayer(long pseudoTickDuration = 0);
        public abstract void PauseLayer();
        public abstract void StopLayer();
        public abstract LayerStatus GetLayerStatus();
    }
}
