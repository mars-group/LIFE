using System;
using LayerAPI.Interfaces;
using LIFE.LayerContainer.LayerAPI.DataTypes;

namespace LayerAPI.AbstractLayers
{
    public abstract class AbstractDistributedSteppedLayer : ISteppedLayer
    {

        public bool InitLayer(LayerInitData layerInitData)
        {
            // TODO: Initialize Agent Stubs etc. based on Distribution Information   
            return true;
        }

        public abstract Guid GetID();
        public abstract void AdvanceOneTick();
        public abstract long GetCurrentTick();
    }
}
