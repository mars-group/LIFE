using System;

using LayerAPI.AbstractLayers;
using LayerAPI.Interfaces;
using LIFE.LayerContainer.LayerAPI.DataTypes;

namespace ExampleLayer
{
    public class ExampleLayer : AbstractDistributedEventDrivenLayer
    {
        public override bool InitLayer(LayerInitData layerInitData)
        {
            throw new NotImplementedException();
        }

        public override void StartLayer(long pseudoTickDuration = 0)
        {
            Console.WriteLine("Hallo Welt");
            Console.ReadLine();
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
