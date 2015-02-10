using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GeoAPI.Geometries;
using Hik.Communication.ScsServices.Service;
using LCConnector.TransportTypes;
using LifeAPI.Layer;
using LIFEViewProtocol.AgentsAndEvents;
using LIFEViewProtocol.Basics;
using LIFEViewProtocol.Helper;
using LIFEViewProtocol.Terrain;
using Mono.Addins;


[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace ExampleLayer
{
    [Extension(typeof(ISteppedLayer))]
    public class ExampleLayer : ScsService, IExampleLayer
    {
        private AgentSmith[] _agents;
        private const int TerrainSizeX = 100;
        private const int TerrainSizeY = 100;
        private const int AgentCount = TerrainSizeX * TerrainSizeY;
        private readonly TerrainDataMessage _terrainMessage = new TerrainDataMessage(TerrainSizeX, TerrainSizeY, 0.0, 1);
        private _2DEnvironment _environment;
        private long _currentTick;

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            Console.WriteLine("Starting initialization...");
            var sw = Stopwatch.StartNew();
            _environment = new _2DEnvironment(TerrainSizeX, TerrainSizeY);
            _agents = new AgentSmith[AgentCount];
            Parallel.For(0, AgentCount, delegate(int i)
            {
                var smith = new AgentSmith(_environment, unregisterAgentHandle, this);
                _agents[i] = smith;
                registerAgentHandle.Invoke(this, smith);
            });

            Console.WriteLine("Initialized {0} agents in {1}ms.", TerrainSizeX * TerrainSizeY, sw.ElapsedMilliseconds);

            return true;
        }

        public void UpdateShadowAgents(IDictionary<Type, List<Guid>> agentsToAdd, IDictionary<Type, List<Guid>> agentsToRemove) {
            
        }


        public long GetCurrentTick()
        {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick)
        {
            _currentTick = currentTick;
        }

        public List<BasicVisualizationMessage> GetVisData()
        {
            var result = new ConcurrentBag<BasicVisualizationMessage> { _terrainMessage };
            Parallel.ForEach(_agents.Where(a => !a.Dead), delegate(AgentSmith a)
            {
                var pos = _environment.GetPosition(a);
                result.Add(new NonMovingBasicAgent(
                    Definitions.AgentTypes.MovingBasicAgent,
                    null,
                    a.AgentID.ToString(),
                    _currentTick,
                    null, "Agentsmith"));
            });

            return result.ToList();
        }

        public List<BasicVisualizationMessage> GetVisData(IGeometry geometry)
        {
            throw new NotImplementedException();
        }


        public void Tick()
        {
            //  Console.WriteLine("I am ExampleLayer and I got ticked");
        }

        public void PreTick()
        {
            //Console.WriteLine("I am ExampleLayer and I got PREticked");
        }

        public void PostTick()
        {
            //Console.WriteLine("I am ExampleLayer and I got POSTticked");
        }
    }
}