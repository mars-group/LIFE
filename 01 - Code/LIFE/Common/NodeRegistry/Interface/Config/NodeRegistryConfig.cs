using System;
using CommonTypes.DataTypes;
using CommonTypes.Types;

namespace NodeRegistry.Interface {
    [Serializable]
    public class NodeRegistryConfig {
        public NodeType NodeType { get; set; }
        public string NodeIdentifier { get; set; }
        public string NodeEndPointIP { get; set; }
        public int NodeEndPointPort { get; set; }
        public int LayerServiceStartPort { get; set; }
        public bool AddMySelfToActiveNodeList { get; set; }
        public int HeartBeatInterval { get; set; }


        public int HeartBeatTimeOutmultiplier { get; set; }

        public NodeRegistryConfig(TNodeInformation nodeinfo, bool addMyselfToActiveNodeList, int heartBeatInterval, int layerServiceStartPort = 39999, int heartbeatTimeoutMultiplier = 3) {
            this.NodeType = nodeinfo.NodeType;
            NodeIdentifier = nodeinfo.NodeIdentifier;
            NodeEndPointIP = nodeinfo.NodeEndpoint.IpAddress;
            NodeEndPointPort = nodeinfo.NodeEndpoint.Port;
            AddMySelfToActiveNodeList = addMyselfToActiveNodeList;
            HeartBeatInterval = heartBeatInterval;
            LayerServiceStartPort = layerServiceStartPort;
            HeartBeatTimeOutmultiplier = heartbeatTimeoutMultiplier;
        }

        public NodeRegistryConfig(NodeType nodeType, string nodeIdentifier, string nodeEndPointIP, int nodeEndPointPort,
            bool myselfToActiveNodeList, int layerServiceStartPort = 39999, int heartBeatInterval = 500, int heartbeatTimeoutMultiplier = 3)
        {
            NodeType = nodeType;
            NodeIdentifier = nodeIdentifier;
            NodeEndPointPort = nodeEndPointPort;
            NodeEndPointIP = nodeEndPointIP;
            AddMySelfToActiveNodeList = myselfToActiveNodeList;
            LayerServiceStartPort = layerServiceStartPort;
            HeartBeatInterval = heartBeatInterval;
            HeartBeatTimeOutmultiplier = heartbeatTimeoutMultiplier;
        }

        public NodeRegistryConfig() {
            LayerServiceStartPort = 39999;

            /*
            NodeType = NodeType.LayerContainer;
            NodeIdentifier = "LC-1";
            NodeEndPointIP = "141.22.11.254";
            NodeEndPointPort = 60100;
             */
            AddMySelfToActiveNodeList = true;
            HeartBeatInterval = 500;
            HeartBeatTimeOutmultiplier = 3;
        }
    }
}