using System.Collections.Generic;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.Scs.Communication.EndPoints.Udp;
using Hik.Communication.Scs.Server;
using Hik.Communication.ScsServices.Client;
using Hik.Communication.ScsServices.Service;
using KNPElevationLayer;
using LCConnector.TransportTypes;
using LifeAPI.Layer;
using LIFEUtilities.MulticastAddressGenerator;
using Mono.Addins;
using TreeLayer.Agents;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace KNPTreeLayer {
    [Extension(typeof (ISteppedLayer))]
    public class TreeLayer : ISteppedLayer {
        private long _currentTick;
        private ElevationLayer _elevationLayer;
        private readonly IScsServiceApplication _server;

        private readonly List<ITree> trees;

        public TreeLayer(ElevationLayer elevationLayer) {
            _elevationLayer = elevationLayer;
            trees = new List<ITree>();
            //Create a Scs Service application that runs on 10048 TCP port.
            _server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(10048));
        }

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {


            foreach (var agentInitConfig in layerInitData.AgentInitConfigs) {
                if (agentInitConfig.AgentName == "Tree") {

                    // instantiate real Agents
                    for (int i = 0; i < agentInitConfig.RealAgentCount; i++) {
                        var t = new Tree(4, 2, 10, 10, 500, 30, 22, agentInitConfig.RealAgentIds[i]);
                        registerAgentHandle(this, t);
                        trees.Add(t);
                        //Add Phone Book Service to service application
                        _server.AddService<ITree, Tree>(t);
                        
                    }

                    // instantiate Shadow Agents
                    for (int i = 0; i < agentInitConfig.ShadowAgentCount; i++) {
                        trees.Add(
                                ScsServiceClientBuilder.CreateClient<ITree>(
                                    new ScsUdpEndPoint(
                                        MulticastAddressGenerator.GetIPv4MulticastAddressByType(typeof (Tree)) + ":6666"),
                                    agentInitConfig.ShadowAgentsIds[i]).ServiceProxy
                                );
                    }

                }
            }

            _server.Start();
            return true;
        }

        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }
    }
}