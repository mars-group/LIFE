using System;

using LayerAPI.Interfaces;
using Mono.Addins;
using ExampleLayer = ExampleLayer.ExampleLayer;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace AwesomeExampleLayer
{
    [Extension(typeof(ISteppedLayer))]
    public class AwesomeExampleLayer : ISteppedLayer
    {
        private global::ExampleLayer.ExampleLayer _layer;

        public AwesomeExampleLayer(global::ExampleLayer.ExampleLayer layer)
        {
            this._layer = layer;
        }

        public long GetCurrentTick()
        {
            throw new System.NotImplementedException();
        }

        public bool InitLayer()
        {
            throw new NotImplementedException();
        }

        public Guid GetID()
        {
            throw new NotImplementedException();
        }
    }
}
