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
    public class NodeRegistryHeartBeatMessage : AbstractNodeRegistryMessage {

        [ProtoMember(20)]
        public String NodeIdentifier;
        [ProtoMember(21)]
        public NodeType NodeType;
       
        private NodeRegistryHeartBeatMessage() {
            
        }

        public NodeRegistryHeartBeatMessage(NodeRegistryMessageType messageType, string nodeIdentifier, NodeType nodeType)
            : base(messageType)
        {
            NodeIdentifier = nodeIdentifier;
            NodeType = nodeType;
        }
    }
}
