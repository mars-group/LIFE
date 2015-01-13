using System;
using System.Collections.Generic;
using System.Linq;
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
        //private IKnpElevationLayer _elevationLayer;

        private readonly List<ITree> trees;
        private readonly AgentShadowingServiceComponent<ITree, Tree> _agentShadowingService;
        private readonly IKnpElevationLayer _elevationLayer;

        public TreeLayer(IKnpElevationLayer elevationLayer)
        {
            _elevationLayer = elevationLayer;
            trees = new List<ITree>();
            _agentShadowingService = new AgentShadowingServiceComponent<ITree, Tree>();
        }

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {


            var env = _elevationLayer.GetEnvelope();
            var randX = new Random();
            var randY = new Random();

            var MinX = 31.331;
            var MinY = -25.292;
            var MaxX = 31.985;
            var MaxY = -24.997;

            foreach (var agentInitConfig in layerInitData.AgentInitConfigs) {
                if (agentInitConfig.AgentName != "Tree") continue;
                // instantiate real Agents



                for (var i = 0; i < agentInitConfig.RealAgentCount; i++) {
                    var t = new Tree(4, 2, 10, i, 500,
                        GetRandomNumber(MinX, MaxX),
                        GetRandomNumber(MinY, MaxY),
                        agentInitConfig.RealAgentIds[i],
                        this,
                        _elevationLayer
                     );
                    registerAgentHandle(this, t);
                    trees.Add(t);
                    _agentShadowingService.RegisterRealAgent(t);
                }

                // instantiate Shadow Agents
                for (int i = 0; i < agentInitConfig.ShadowAgentCount; i++) {
                    trees.Add(_agentShadowingService.CreateShadowAgent(agentInitConfig.ShadowAgentsIds[i]));
                }
            }
            return true;
        }

        private double GetRandomNumber(double minimum, double maximum)
        { 
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        internal List<ITree> GetAllOtherTreesThanMe(ITree memyself)
        {
            return trees.FindAll(t => t != memyself);
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