﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using CommonTypes.DataTypes;

namespace NodeRegistry.Implementation.Messages
{
    public class NodeRegistryConnectionInfoMessage : AbstractNodeRegistryMessage
    {
        public string OriginAddress { get; set; }


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