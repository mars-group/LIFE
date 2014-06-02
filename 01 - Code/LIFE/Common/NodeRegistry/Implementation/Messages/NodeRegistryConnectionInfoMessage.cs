using System.Net;
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

        public NodeRegistryConnectionInfoMessage(NodeRegistryMessageType messageType, TNodeInformation information, string address)
            : base(messageType) {
                OriginAddress = address;
            NodeInformation = information;
        }
    }
}