using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentShadowingService.Implementation;
using AgentShadowingService.Interface;
using Hik.Communication.ScsServices.Service;
using KNPElevationLayer;
using KNPEnvironmentLayer;
using LCConnector.Exceptions;
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
        private readonly List<ITree> trees;
        private readonly AgentShadowingServiceComponent<ITree, Tree> _agentShadowingService;
        private readonly IKnpElevationLayer _elevationLayer;
        private readonly List<ITree> _agentsToRemoveInPostTick;
        private readonly List<ITree> _agentsToAddInPostTick;

        private double MinX = 31.331;
        private double MinY = -25.292;
        private double MaxX = 31.985;
        private double MaxY = -24.997;
        private UnregisterAgent _unregisterAgentHandle;
        private IKNPEnvironmentLayer _environmentLayer;
        private ConcurrentDictionary<Guid, ITree> _localTreeMap;

        public TreeLayer(IKnpElevationLayer elevationLayer, IKNPEnvironmentLayer knpEnvironmentLayer)
        {
            _elevationLayer = elevationLayer;
            _environmentLayer = knpEnvironmentLayer;
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

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            _unregisterAgentHandle = unregisterAgentHandle;

            foreach (var agentInitConfig in layerInitData.AgentInitConfigs) {
                if (agentInitConfig.AgentName != "Tree") continue;
                var agentBag = new ConcurrentBag<Tree>();
                _localTreeMap = new ConcurrentDictionary<Guid, ITree>();
                // instantiate real Agents
                var config = agentInitConfig;
                var placementError = false;
                Parallel.For(0, agentInitConfig.RealAgentCount, i => {
                    var t = new Tree(4, 2, 10, i, 500,
                        GetRandomDouble(MinX, MaxX),
                        GetRandomDouble(MinY, MaxY),
                        config.RealAgentIds[i],
                        this,
                        _elevationLayer,
                        _environmentLayer
                    );

                    // add to ESC 
                    if (!_environmentLayer.Add(t.SpatialEntity, t.SpatialEntity.Shape.Position)) placementError = true;
                        _localTreeMap.TryAdd(config.RealAgentIds[i], t);
                    registerAgentHandle(this, t);
                });

                if (placementError) return false;

                Console.WriteLine("Finished: Realagents instantiated.");

                if (layerInitData.Distribute)
                {
                    _agentShadowingService.RegisterRealAgents(agentBag.ToArray());
                }

                Console.WriteLine("Finished: Realagents registered.");

                if (layerInitData.Distribute) {
                    // instantiate Shadow Agents
                    _agentShadowingService.CreateShadowAgents(agentInitConfig.ShadowAgentsIds);
                }

                Console.WriteLine("Finished: ShadowAgents created.");
            }
            return true;
        }

        public double ChopTree(Guid id) {
            ITree choppedTree;
            if (_localTreeMap.TryRemove(id, out choppedTree)) {
                _unregisterAgentHandle(this,choppedTree);
                return choppedTree.Biomass;
            }
            return 0;
        }

        public bool GetTreeById(Guid id, out ITree tree)
        {
            return _localTreeMap.TryGetValue(id, out tree);
        }

        private double GetRandomDouble(double minimum, double maximum)
        { 
            var random = new Random(28937892);
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }


    }
}