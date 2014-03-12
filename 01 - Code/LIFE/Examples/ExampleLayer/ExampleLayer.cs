using System;
using LayerAPI.AbstractLayers;
using LayerAPI.DataTypes;
using LayerAPI.Interfaces;
using Mono.Addins;

[assembly:Addin]
[assembly:AddinDependency("LayerContainer", "0.1")]
namespace ExampleLayer
{

    [Extension(typeof(IEventDrivenLayer))]
    public class ExampleLayer : AbstractDistributedSteppedLayer
    {

        public override void AdvanceOneTick()
        {
            throw new NotImplementedException();
        }

        public override long GetCurrentTick()
        {
            throw new NotImplementedException();
        }
    }


}
