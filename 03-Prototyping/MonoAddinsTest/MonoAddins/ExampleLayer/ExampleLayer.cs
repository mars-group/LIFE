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

        public ExampleLayer()
        {
            _agent = new AgentSmith();
        }

        public bool InitLayer()
        {
            throw new NotImplementedException();
        }

        public Guid GetID()
        {
            throw new NotImplementedException();
        }

        public long GetCurrentTick()
        {
            throw new NotImplementedException();
        }
    }


}
