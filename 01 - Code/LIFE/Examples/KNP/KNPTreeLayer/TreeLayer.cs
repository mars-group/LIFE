﻿using System.Collections.Generic;
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
                    // evil hack only for testing purposes
                    bool sendingNote = _agentShadowingService.GetLayerContainerName() == "LC-1";
                    // instantiate real Agents
                    for (int i = 0; i < agentInitConfig.RealAgentCount; i++) {
                        var t = new Tree(4, 2, 10, i, 500, 30, 22, agentInitConfig.RealAgentIds[i], _elevationLayer, this, sendingNote);
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