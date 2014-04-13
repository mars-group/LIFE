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
        public bool AddMySelfToActiveNodeList { get; set; }
        public int HeartBeatInterval { get; set; }

        public NodeRegistryConfig(NodeInformationType nodeinfo, bool addMyselfToActiveNodeList, int heartBeatInterval) {
            this.NodeType = nodeinfo.NodeType;
            NodeIdentifier = nodeinfo.NodeIdentifier;
            NodeEndPointIP = nodeinfo.NodeEndpoint.IpAddress;
            NodeEndPointPort = nodeinfo.NodeEndpoint.Port;
            AddMySelfToActiveNodeList = addMyselfToActiveNodeList;
            HeartBeatInterval = heartBeatInterval;
        }

        public NodeRegistryConfig(NodeType nodeType, string nodeIdentifier, string nodeEndPointIP, int nodeEndPointPort,
            bool myselfToActiveNodeList, int heartBeatInterval = 500) {
            NodeType = nodeType;
            NodeIdentifier = nodeIdentifier;
            NodeEndPointPort = nodeEndPointPort;
            NodeEndPointIP = nodeEndPointIP;
            AddMySelfToActiveNodeList = myselfToActiveNodeList;
            HeartBeatInterval = heartBeatInterval;
        }

        public NodeRegistryConfig() {
            NodeType = NodeType.LayerContainer;
            NodeIdentifier = "LC-1";
            NodeEndPointIP = "141.22.11.254";
            NodeEndPointPort = 60100;
            AddMySelfToActiveNodeList = true;
            HeartBeatInterval = 500;
        }
    }
}