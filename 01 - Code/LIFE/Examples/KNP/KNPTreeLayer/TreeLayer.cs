﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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


        public TreeLayer() {
            trees = new List<ITree>();
            _agentShadowingService = new AgentShadowingServiceComponent<ITree, Tree>();
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

            var MinX = 31.331;
            var MinY = -25.292;
            var MaxX = 31.985;
            var MaxY = -24.997;

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
                    //_agentShadowingService.RegisterRealAgent(t);
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