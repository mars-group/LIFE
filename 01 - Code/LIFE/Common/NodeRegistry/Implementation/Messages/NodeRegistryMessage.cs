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
        private NodeRegistryMessageType messageType;
        [ProtoMember(2)]
        private NodeInformationType nodeInformationType;


        public NodeRegistryMessage(NodeRegistryMessageType messageType, NodeInformationType informationType)
        {

            this.messageType = messageType;
            this.nodeInformationType = informationType;
        }
    }

    
}
