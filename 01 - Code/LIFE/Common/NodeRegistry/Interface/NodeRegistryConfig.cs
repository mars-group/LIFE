﻿using System;
using CommonTypes.Types;
using ConfigurationAdapter.Interface.Exceptions;

namespace NodeRegistry.Interface
{
    [Serializable]
    public class NodeRegistryConfig
    {



        public NodeType NodeType { get; set; }
        public string NodeIdentifier { get; set; }
        public string NodeEndPointIP { get; set; }
        public int NodeEndPointPort { get; set; }
        public bool AddMySelfToActiveNodeList { get; set; }

        public NodeRegistryConfig(NodeType nodeType, string nodeIdentifier, string nodeEndPointIP, int nodeEndPointPort, bool myselfToActiveNodeList)
        {
            NodeType = nodeType;
            NodeIdentifier = nodeIdentifier;
            NodeEndPointPort = nodeEndPointPort;
            NodeEndPointIP = nodeEndPointIP;
            AddMySelfToActiveNodeList = myselfToActiveNodeList;
        }

        public NodeRegistryConfig()
        {
            NodeType = NodeType.LayerContainer;
            NodeIdentifier = "IamSoMadRightNow";
            NodeEndPointIP = "141.22.11.254";
            NodeEndPointPort = 60100;
            AddMySelfToActiveNodeList = true;
        }
    }
}
