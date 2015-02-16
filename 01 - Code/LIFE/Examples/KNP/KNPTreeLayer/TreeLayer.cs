using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentShadowingService.Implementation;
using AgentShadowingService.Interface;
using GeoAPI.Geometries;
using Hik.Communication.ScsServices.Service;
using KNPElevationLayer;
using LCConnector.TransportTypes;
using LifeAPI.Layer;
using Mono.Addins;
using SpatialAPI.Environment;
using TreeLayer;
using TreeLayer.Agents;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace KNPTreeLayer {
    [Extension(typeof (ISteppedLayer))]
    public class TreeLayer : ScsService, IKnpTreeLayer
    {
        private long _currentTick;

        private readonly List<ITree> _trees;
        private readonly AgentShadowingServiceComponent<ITree, Tree> _agentShadowingService;
        private readonly IKnpElevationLayer _elevationLayer;
        private readonly List<ITree> _agentsToRemoveInPostTick;
        private readonly List<ITree> _agentsToAddInPostTick;
        private readonly Random _random = new Random();

        private double MinX = 31.331;
        private double MinY = -25.292;
        private double MaxX = 31.985;
        private double MaxY = -24.997;
        private UnregisterAgent _unregisterAgentHandle;
        private IEnvironment _environment;

        public TreeLayer(IKnpElevationLayer elevationLayer) {
          _elevationLayer = elevationLayer;
          
          // Create the environment. It works with an positive extent.
          var envelope = elevationLayer.GetEnvelope();
          Coordinate coord = elevationLayer.TransformToImage(envelope.MaxX, envelope.MaxY);
          _environment = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();


          _agentsToRemoveInPostTick = new List<ITree>();
          _agentsToAddInPostTick = new List<ITree>();
          _agentShadowingService = new AgentShadowingServiceComponent<ITree, Tree>();
          _agentShadowingService.AgentUpdates += OnAgentUpdates;
          _trees = new List<ITree>();
        }

        private void OnAgentUpdates(object sender, LIFEAgentEventArgs<ITree> e) {
            _agentsToRemoveInPostTick.AddRange(e.RemovedAgents);
            _agentsToAddInPostTick.AddRange(e.NewAgents);
        }


        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            _unregisterAgentHandle = unregisterAgentHandle;

            foreach (var agentInitConfig in layerInitData.AgentInitConfigs) {
                if (agentInitConfig.AgentName != "Tree") continue;
                var agentBag = new ConcurrentBag<Tree>();
                // instantiate real Agents
                var config = agentInitConfig;
                Parallel.For(0, agentInitConfig.RealAgentCount, i => {
                  var t = new Tree(4, 2, 10, i, 500, 
                    GetRandomNumber(0, _environment.MaxDimension.X), 
                    GetRandomNumber(0, _environment.MaxDimension.Y), 
                    config.RealAgentIds[i], this,
                    _elevationLayer, registerAgentHandle, unregisterAgentHandle, _environment
                  );
                  agentBag.Add(t);
                });

                Console.WriteLine("Finished: Realagents instantiated.");

                if (layerInitData.Distribute)
                {
                    _agentShadowingService.RegisterRealAgents(agentBag.ToArray());
                }

                Console.WriteLine("Finished: Realagents registered.");

                _trees.AddRange(agentBag);
               
                if (layerInitData.Distribute) {
                    // instantiate Shadow Agents
                    var shadowTrees = _agentShadowingService.CreateShadowAgents(agentInitConfig.ShadowAgentsIds);
                    foreach (var shadowTree in shadowTrees) {
                        //_environment.Add(shadowTree)
                    }
                }

                Console.WriteLine("Finished: ShadowAgents created.");
            }
            return true;
        }


        public void PreTick() {
            /*
            // remove an agent
            var tRemove = trees[0];
            _unregisterAgentHandle.Invoke(this, tRemove);
            trees.RemoveAt(0);
            _agentShadowingService.RemoveRealAgent((Tree)tRemove);
            // create new agent
            var t = new Tree(4, 2, 10, 7, 500,
                        GetRandomNumber(MinX, MaxX),
                        GetRandomNumber(MinY, MaxY),
                        Guid.NewGuid(),
                        this
                    );
            trees.Add(t);
            _agentShadowingService.CreateShadowAgent(t.ID);
             * */
        }

        public void Tick() {
            Console.WriteLine("Agents present on this node: " + _trees.Count);
        }


        public void PostTick() {
            // update internal agent lists with information from AgentUpdates event
            //trees.RemoveAll(t => _agentsToRemoveInPostTick.Contains(t));
            //trees.AddRange(_agentsToAddInPostTick);
            // since the added agents are all ShadowAgents, we don't need to register them for execution
        }


        private double GetRandomNumber(double minimum, double maximum) {        
          return _random.NextDouble() * (maximum - minimum) + minimum;
        }


        internal ITree GetOneOtherTreesThanMe(ITree memyself)
        {
            return _trees.Find(t => t != memyself);
        }

        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }

        public string Name
        {
            get { return "TreeLayer"; }
        }

        public ITree GetTreeById(Guid id) {
            return _trees.Find(t => t.ID == id);
        }
    }
}