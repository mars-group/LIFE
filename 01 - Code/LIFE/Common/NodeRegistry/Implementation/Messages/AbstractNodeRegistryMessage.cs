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

        public AbstractNodeRegistryMessage(){}
        
        protected AbstractNodeRegistryMessage(NodeRegistryMessageType messageType) {
            MessageType = messageType;
        }
    }
}
