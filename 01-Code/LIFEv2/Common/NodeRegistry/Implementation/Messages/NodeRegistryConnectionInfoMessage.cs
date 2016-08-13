//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using CommonTypes.DataTypes;
using ProtoBuf;

namespace NodeRegistry.Implementation.Messages
{
    [ProtoContract]
    public class NodeRegistryConnectionInfoMessage : AbstractNodeRegistryMessage
    {
        [ProtoMember(10)]
        public string OriginAddress { get; set; }


        [ProtoMember(20)]
        public TNodeInformation NodeInformation { get; set; }


        public NodeRegistryConnectionInfoMessage()
            : base()
        {
        }

        public NodeRegistryConnectionInfoMessage(NodeRegistryMessageType messageType, TNodeInformation information, string address, string clusterName)
            : base(messageType, clusterName) {
                OriginAddress = address;
            NodeInformation = information;
        }
    }
}