using System.Collections.Generic;
using AgentShadowingService.Implementation;
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
        private IKnpElevationLayer _elevationLayer;

        private readonly List<ITree> trees;
        private readonly AgentShadowingServiceComponent<ITree, Tree> _agentShadowingService;

        public TreeLayer(IKnpElevationLayer elevationLayer)
        {
            _elevationLayer = elevationLayer;
            trees = new List<ITree>();
            _agentShadowingService = new AgentShadowingServiceComponent<ITree, Tree>();
        }

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {


            foreach (var agentInitConfig in layerInitData.AgentInitConfigs) {
                if (agentInitConfig.AgentName == "Tree") {

                    // instantiate real Agents
                    for (int i = 0; i < agentInitConfig.RealAgentCount; i++) {
                        var t = new Tree(4, 2, 10, 10, 500, 30, 22, agentInitConfig.RealAgentIds[i]);
                        registerAgentHandle(this, t);
                        trees.Add(t);
                        _agentShadowingService.RegisterRealAgent(t);
                    }

                    // instantiate Shadow Agents
                    for (int i = 0; i < agentInitConfig.ShadowAgentCount; i++) {
                        trees.Add(_agentShadowingService.CreateShadowAgent(agentInitConfig.ShadowAgentsIds[i]));
                    }

                }
            }
            return true;
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