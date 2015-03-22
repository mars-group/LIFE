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
        private readonly AgentShadowingServiceComponent<ITree, Tree> _agentShadowingService;
        private readonly IKnpElevationLayer _elevationLayer;
        private readonly List<ITree> _agentsToRemoveInPostTick;
        private readonly List<ITree> _agentsToAddInPostTick;

        private double MinX = 31.331;
        private double MinY = -25.292;
        private double MaxX = 31.985;
        private double MaxY = -24.997;
        private UnregisterAgent _unregisterAgentHandle;
        private readonly IKNPEnvironmentLayer _environmentLayer;
        private ConcurrentDictionary<Guid, ITree> _localTreeMap;

        public TreeLayer(IKnpElevationLayer elevationLayer)
        {
            _elevationLayer = elevationLayer;
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
                Parallel.For(0, agentInitConfig.RealAgentCount, i =>
                {

                    var clusterGroup = new Guid[4];
                    // create interaction cluster group
                    if (i == 0)
                    {
                        clusterGroup[0] = config.RealAgentIds[config.RealAgentCount - 1];
                        clusterGroup[1] = config.RealAgentIds[i + 1];
                        if (layerInitData.Distribute)
                        {
                            clusterGroup[2] = config.ShadowAgentsIds[config.RealAgentCount - 1];
                            clusterGroup[3] = config.ShadowAgentsIds[i + 1];
                        }
                        else
                        {
                            clusterGroup[2] = config.RealAgentIds[config.RealAgentCount - 2];
                            clusterGroup[3] = config.RealAgentIds[i + 2];
                        }
                    }
                    else if (i == 1 && !layerInitData.Distribute)
                    {
                        clusterGroup[0] = config.RealAgentIds[i - 1];
                        clusterGroup[1] = config.RealAgentIds[i + 1];
                        clusterGroup[2] = config.RealAgentIds[config.RealAgentCount - 1];
                        clusterGroup[3] = config.RealAgentIds[i + 2];
                    }
                    else if (i == config.RealAgentCount-2 && !layerInitData.Distribute)
                    {
                        clusterGroup[0] = config.RealAgentIds[i - 1];
                        clusterGroup[1] = config.RealAgentIds[i + 1];
                        clusterGroup[2] = config.RealAgentIds[i - 2];
                        clusterGroup[3] = config.RealAgentIds[0];
                    }
                    else if (i == config.RealAgentCount - 1)
                    {
                        clusterGroup[0] = config.RealAgentIds[i - 1];
                        clusterGroup[1] = config.RealAgentIds[0];
                        if (layerInitData.Distribute)
                        {
                            clusterGroup[2] = config.ShadowAgentsIds[i - 1];
                            clusterGroup[3] = config.ShadowAgentsIds[0];
                        }
                        else
                        {
                            clusterGroup[2] = config.RealAgentIds[i - 2];
                            clusterGroup[3] = config.RealAgentIds[1];
                        }
                    }
                    else
                    {
                        clusterGroup[0] = config.RealAgentIds[i - 1];
                        clusterGroup[1] = config.RealAgentIds[i + 1];
                        if (layerInitData.Distribute)
                        {
                            clusterGroup[2] = config.ShadowAgentsIds[i - 1];
                            clusterGroup[3] = config.ShadowAgentsIds[i + 1];
                        }
                        else
                        {
                            clusterGroup[2] = config.RealAgentIds[i - 2];
                            clusterGroup[3] = config.RealAgentIds[i + 2];
                        }
                    }


                    var t = new Tree(4, 2, 10, i, 500,
                        GetRandomDouble(MinX, MaxX),
                        GetRandomDouble(MinY, MaxY),
                        config.RealAgentIds[i],
                        this,
                        _elevationLayer,
                        clusterGroup
                    );

                    _localTreeMap.TryAdd(config.RealAgentIds[i], t);
                    registerAgentHandle(this, t);
                });

                Console.WriteLine("Finished: Realagents instantiated.");

                if (layerInitData.Distribute)
                {
                    _agentShadowingService.RegisterRealAgents(agentBag.ToArray());
                    Console.WriteLine("Finished: Realagents registered.");
                }

                if (layerInitData.Distribute) {
                    // instantiate Shadow Agents
                    var shadowTrees = _agentShadowingService.CreateShadowAgents(agentInitConfig.ShadowAgentsIds).ToArray();
                    for (int i = 0; i < agentInitConfig.ShadowAgentsIds.Length; i++) {
                        _localTreeMap.TryAdd(agentInitConfig.ShadowAgentsIds[i], shadowTrees[i]);
                    }
                    Console.WriteLine("Finished: ShadowAgents created.");
                }

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