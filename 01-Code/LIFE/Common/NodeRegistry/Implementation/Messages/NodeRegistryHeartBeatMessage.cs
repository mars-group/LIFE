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
using System.Text;
using System.Threading.Tasks;
using CommonTypes.Types;
using ProtoBuf;

namespace NodeRegistry.Implementation.Messages
{
    [ProtoContract]
    public class NodeRegistryHeartBeatMessage : AbstractNodeRegistryMessage
    {

        [ProtoMember(20)]
        public String NodeIdentifier;
        [ProtoMember(21)]
        public NodeType NodeType;

        public NodeRegistryHeartBeatMessage()
            : base()
        {
        }



        public NodeRegistryHeartBeatMessage(NodeRegistryMessageType messageType, string nodeIdentifier, NodeType nodeType)
            : base(messageType)
        {
            NodeIdentifier = nodeIdentifier;
            NodeType = nodeType;
        }
    }
}
