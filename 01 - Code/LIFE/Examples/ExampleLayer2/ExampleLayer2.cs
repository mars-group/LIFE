using System;
using System.Collections.Generic;
using ExampleLayer;
using LayerAPI.Interfaces;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace ExampleLayer2 {
    [Extension(typeof (ISteppedLayer))]
    public class ExampleLayer2 : ISteppedLayer {
        private readonly List<AgentSmith> _agents;

        private const int agentCount = 10000;

        public ExampleLayer2() {
            var _environment = new _2DEnvironment(100,100);
            _agents = new List<AgentSmith>();
            for (var i = 0; i < agentCount; i++) { _agents.Add(new AgentSmith(_environment)); }
            _environment.RandomlyAddAgentsToFreeFields(_agents);
        }

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle) {
            foreach (var agentSmith in _agents) {
                registerAgentHandle.Invoke(this, agentSmith);  
            }

            return true;
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }
    }
}