using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GeoAPI.Geometries;
using LayerAPI.Interfaces;
using LayerAPI.Interfaces.Visualization;
using MessageWrappers;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace ExampleLayer {
    using System.Collections.Generic;

    [Extension(typeof (ISteppedLayer))]
    public class ExampleLayer : ISteppedActiveLayer, IVisualizable {

		private List<AgentSmith> _agents;
        private const int agentCount = 10000;

        public ExampleLayer() {

        }

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            var _environment = new _2DEnvironment(100, 100);
            _agents = new List<AgentSmith>();
            for (var i = 0; i < agentCount; i++) { _agents.Add(new AgentSmith(_environment, unregisterAgentHandle, this)); }
            
            //_environment.RandomlyAddAgentsToFreeFields(_agents);

            foreach (var agentSmith in _agents) {
               registerAgentHandle.Invoke(this, agentSmith);  
            }

            return true;
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }

        public List<BasicVisualizationMessage> GetVisData() {
            var result = new ConcurrentBag<BasicVisualizationMessage>();
            result.Add(new TerrainDataMessage(100, 0, 100));
            Parallel.ForEach(_agents, a => result.Add(new BasicAgent() {
                Id = a.AgentID.ToString(),
                Description = "AgentSmith",
                State = a.Dead ? "Dead" : "Alive",
                X =  a.MyPosition.X,
                Y = a.MyPosition.Y
            }));
            return result.ToList();
        }

        public List<BasicVisualizationMessage> GetVisData(IGeometry geometry) {
            throw new NotImplementedException();
        }


        public void Tick() {
            Console.WriteLine("I am ExampleLayer and I got ticked");
        }

        public void PreTick() {
            Console.WriteLine("I am ExampleLayer and I got PREticked");
        }

        public void PostTick() {
            Console.WriteLine("I am ExampleLayer and I got POSTticked");
        }
    }
}