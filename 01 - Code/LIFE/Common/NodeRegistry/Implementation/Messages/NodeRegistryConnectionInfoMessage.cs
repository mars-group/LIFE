using System.Net;
using CommonTypes.DataTypes;
using ProtoBuf;

namespace NodeRegistry.Implementation.Messages
{
    [ProtoContract]
    public class NodeRegistryConnectionInfoMessage : AbstractNodeRegistryMessage
    {

        //TODO IP Addresse in NodeConnectionInfo einziehen
        // public IPAddress Address { get { return new IPAddress(_address); } set { _address = value.GetAddressBytes(); } }


        [ProtoMember(10)]
        public TNodeInformation NodeInformation { get; set; }

        public NodeRegistryConnectionInfoMessage()
            : base()
        {
        }

        public NodeRegistryConnectionInfoMessage(NodeRegistryMessageType messageType, TNodeInformation information)
            : base(messageType)
        {

            NodeInformation = information;
        }
    }
}