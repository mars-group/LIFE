//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using ProtoBuf;

namespace NodeRegistry.Implementation.Messages
{

    [ProtoContract]
    [ProtoInclude(1, typeof(NodeRegistryConnectionInfoMessage))]
    [ProtoInclude(2, typeof(NodeRegistryHeartBeatMessage))]
    public abstract class AbstractNodeRegistryMessage
    {

        [ProtoMember(3)]
        public NodeRegistryMessageType MessageType;

        [ProtoMember(4)]
        public string ClusterName { get; set; }

        public AbstractNodeRegistryMessage()
        {
            
        }

        protected AbstractNodeRegistryMessage(NodeRegistryMessageType messageType, string clusterName)
        {
            MessageType = messageType;
            ClusterName = clusterName;
        }
    }
}
