using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GeoAPI.Geometries;
using LayerAPI.Layer;
using LayerAPI.Layer.Visualization;
using MessageWrappers;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace ExampleLayer
{
    using System.Collections.Generic;

    [Mono.Addins.Extension(typeof(ISteppedLayer))]
    public class ExampleLayer : ISteppedActiveLayer, IVisualizable
    {
        private AgentSmith[] _agents;
        private const int TerrainSizeX = 300;
        private const int TerrainSizeY = 200;
        private const int AgentCount = TerrainSizeX * TerrainSizeY;
        private readonly TerrainDataMessage _terrainMessage = new TerrainDataMessage(TerrainSizeX, TerrainSizeY, 0);
        private _2DEnvironment _environment;
        private long _currentTick;

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
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

        public long GetCurrentTick()
        {
            throw new NotImplementedException();
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
                result.Add(new BasicAgent()
                {
                    Id = a.AgentID.ToString(),
                    Description = "AgentSmith",
                    State = a.Dead ? "Dead" : "Alive",
                    X = (float)pos.X,
                    Y = (float)pos.Y
                });
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