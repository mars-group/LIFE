using System;
using LayerAPI.AbstractLayers;
using LayerAPI.DataTypes;
using LayerAPI.Interfaces;
using Mono.Addins;

[assembly:Addin]
[assembly:AddinDependency("LayerContainer", "0.1")]
namespace ExampleLayer
{

    [Extension(typeof(ISteppedLayer))]
    public class ExampleLayer : AbstractDistributedSteppedLayer
    {
        private readonly AgentSmith _agent;

        public ExampleLayer() : base()
        {
            _agent = new AgentSmith();
        }

        public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            registerAgentHandle.Invoke(_agent);
            return true;
        }

        public override long GetCurrentTick()
        {
            throw new NotImplementedException();
        }
    }


}
