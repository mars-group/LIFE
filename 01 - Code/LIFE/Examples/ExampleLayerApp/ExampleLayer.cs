using System;

using LayerAPI.AbstractLayers;
using LayerAPI.Interfaces;

namespace ExampleLayerApp
{
    public class ExampleLayer : AbstractDistributedEventDrivenLayer
    {
        public override void StartLayer(long startTime = 0, long pseudoTickDuration = 0)
        {
            throw new NotImplementedException();
        }

        public override void PauseLayer()
        {
            throw new NotImplementedException();
        }

        public override void StopLayer()
        {
            throw new NotImplementedException();
        }

        public override LayerStatus GetLayerStatus()
        {
            throw new NotImplementedException();
        }
    }
}
