using System;
using LayerAPI.Interfaces;
using Mono.Addins;

[assembly:Addin]
[assembly:AddinDependency("LayerContainer", "0.1")]
namespace ExampleLayer
{

    [Extension(typeof(ISteppedLayer))]
    public class ExampleLayer : ISteppedLayer
    {
        private readonly AgentSmith _agent;

        public ExampleLayer() : base()
        {
            _agent = new AgentSmith();
        }

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            registerAgentHandle.Invoke(this, _agent);
            return true;
        }

        public long GetCurrentTick()
        {
            throw new NotImplementedException();
        }
    }


}
