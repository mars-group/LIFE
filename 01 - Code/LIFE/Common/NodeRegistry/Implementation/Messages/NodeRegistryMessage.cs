using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using ProtoBuf;

namespace NodeRegistry.Implementation.Messages
{
    [ProtoContract]
    class NodeRegistryMessage
    {
        [ProtoMember(1)]
        public NodeRegistryMessageType messageType { get; private set; }


        [ProtoMember(2)]
        public NodeInformationType nodeInformationType { get; private set; }


        private NodeRegistryMessage()
        {
            
        }

        public NodeRegistryMessage(NodeRegistryMessageType messageType, NodeInformationType informationType)
        {
            this.messageType = messageType;
            this.nodeInformationType = informationType;
        }
    }


}
