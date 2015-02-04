using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentShadowingService.Implementation;
using AgentShadowingService.Interface;
using Hik.Communication.ScsServices.Service;
using KNPElevationLayer;
using LCConnector.TransportTypes;
using LifeAPI.Layer;
using Mono.Addins;
using TreeLayer;
using TreeLayer.Agents;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace KNPTreeLayer {
    [Extension(typeof (ISteppedLayer))]
    public class TreeLayer : ScsService, IKnpTreeLayer
    {
        private long _currentTick;
        //private IKnpElevationLayer _elevationLayer;

        private readonly List<ITree> trees;
        private readonly AgentShadowingServiceComponent<ITree, Tree> _agentShadowingService;
        private readonly IKnpElevationLayer _elevationLayer;
        private List<ITree> _agentsToRemoveInPostTick;
        private List<ITree> _agentsToAddInPostTick;

        private double MinX = 31.331;
        private double MinY = -25.292;
        private double MaxX = 31.985;
        private double MaxY = -24.997;
        private UnregisterAgent _unregisterAgentHandle;

        public TreeLayer() {
            trees = new List<ITree>();
            _agentsToRemoveInPostTick = new List<ITree>();
            _agentsToAddInPostTick = new List<ITree>();
            _agentShadowingService = new AgentShadowingServiceComponent<ITree, Tree>();
            _agentShadowingService.AgentUpdates += OnAgentUpdates;
        }

        private void OnAgentUpdates(object sender, LIFEAgentEventArgs<ITree> e) {
            _agentsToRemoveInPostTick.AddRange(e.RemovedAgents);
            _agentsToAddInPostTick.AddRange(e.NewAgents);
        }

        /*
        public TreeLayer(IKnpElevationLayer elevationLayer)
        {
            _elevationLayer = elevationLayer;
            trees = new List<ITree>();
            _agentShadowingService = new AgentShadowingServiceComponent<ITree, Tree>();
        }
        */
        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            _unregisterAgentHandle = unregisterAgentHandle;


            foreach (var agentInitConfig in layerInitData.AgentInitConfigs) {
                if (agentInitConfig.AgentName != "Tree") continue;
                var agentBag = new ConcurrentBag<Tree>();
                // instantiate real Agents
                var config = agentInitConfig;
                Parallel.For(0, agentInitConfig.RealAgentCount, i => {
                    var t = new Tree(4, 2, 10, i, 500,
                        GetRandomNumber(MinX, MaxX),
                        GetRandomNumber(MinY, MaxY),
                        config.RealAgentIds[i],
                        this
                        //_elevationLayer
                    );
                    agentBag.Add(t);
                    registerAgentHandle(this, t);
                });

                Console.WriteLine("Finished: Realagents instantiated.");

                if (layerInitData.Distribute)
                {
                    _agentShadowingService.RegisterRealAgents(agentBag.ToArray());
                }

                Console.WriteLine("Finished: Realagents registered.");

                trees.AddRange(agentBag);
               
                if (layerInitData.Distribute) {
                    // instantiate Shadow Agents
                    _agentShadowingService.CreateShadowAgents(agentInitConfig.ShadowAgentsIds);
                }

                Console.WriteLine("Finished: ShadowAgents created.");
            }
            return true;
        }

        public void UpdateShadowAgents(IDictionary<Type, List<Guid>> agentsToAdd, IDictionary<Type, List<Guid>> agentsToRemove) {
            foreach (var id in agentsToAdd[typeof(Tree)]) {
                trees.Add(_agentShadowingService.CreateShadowAgent(id));
            }

            foreach (var id in agentsToRemove[typeof(Tree)])
            {
                _agentShadowingService.CreateShadowAgent(id);
            }
        }


        public void PreTick() {
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
        }

        public void Tick() {
            Console.WriteLine("Agents present on this node: " + trees.Count);
        }


        public void PostTick() {
            // update internal agent lists with information from AgentUpdates event
            trees.RemoveAll(t => _agentsToRemoveInPostTick.Contains(t));
            trees.AddRange(_agentsToAddInPostTick);
            // since the added agents are all ShadowAgents, we don't need to register them for execution
        }


        private double GetRandomNumber(double minimum, double maximum)
        { 
            var random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        internal ITree GetOneOtherTreesThanMe(ITree memyself)
        {
            return trees.Find(t => t != memyself);
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

    }
}