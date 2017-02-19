//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
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

        /// <summary>
        /// Empty constructor for reflektion purpose only. 
        /// Do not use this constructor unless you know what you are doing.
        /// </summary>
        private NodeRegistryConfig() {
            
        }
      
    }
}