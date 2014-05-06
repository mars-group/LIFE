using NodeRegistry.Interface;

namespace SimulationController
{
    using System;

    using CommonTypes.Types;



    [Serializable]
    public class SimControllerConfig
    {
        public NodeRegistryConfig NodeRegistryConfig { get; set; }


        public SimControllerConfig() {
            this.NodeRegistryConfig = new NodeRegistryConfig();    
        }

        public SimControllerConfig(NodeRegistryConfig nodeRegistryConfig) {
            this.NodeRegistryConfig = nodeRegistryConfig;
        }
        
        public SimControllerConfig(NodeType nodeType, string nodeIdentifier, string nodeEndPointIP, int nodeEndPointPort, bool myselfToActiveNodeList)
        {
            NodeRegistryConfig.NodeType = nodeType;
            NodeRegistryConfig.NodeIdentifier = nodeIdentifier;
            NodeRegistryConfig.NodeEndPointPort = nodeEndPointPort;
            NodeRegistryConfig.NodeEndPointIP = nodeEndPointIP;
            NodeRegistryConfig.AddMySelfToActiveNodeList = myselfToActiveNodeList;
        }
    }
}
