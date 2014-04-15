using System.Net;
using CommonTypes.DataTypes;
using ProtoBuf;

namespace NodeRegistry.Implementation.Messages {
    [ProtoContract]
    internal class NodeRegistryConnectionInfoMessage  : AbstractNodeRegistryMessage {
    
        //TODO IP Addresse in NodeConnectionInfo einziehen
       // public IPAddress Address { get { return new IPAddress(_address); } set { _address = value.GetAddressBytes(); } }

     
        [ProtoMember(10)]
        public NodeInformationType nodeInformationType { get; private set; }
        
        private NodeRegistryConnectionInfoMessage() {
            
        }

        public NodeRegistryConnectionInfoMessage(NodeRegistryMessageType messageType, NodeInformationType informationType) : base(messageType){
            
            nodeInformationType = informationType;
        }
    }
}