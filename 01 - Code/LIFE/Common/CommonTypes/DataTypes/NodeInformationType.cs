
using System.CodeDom;
using System.Security.Cryptography;
using CommonTypes.Types;
using ProtoBuf;

namespace CommonTypes.DataTypes
{

    [ProtoContract]
    public class NodeInformationType
    {
        [ProtoMember(1)]
        public NodeType NodeType { get; private set; }
        
        [ProtoMember(2)]
        public string NodeIdentifier { get; private set; }

        [ProtoMember(3)]
        public NodeEndpoint NodeEndpoint { get; private set; }


        private NodeInformationType()
        {
            
        }

        public NodeInformationType(NodeType nodeType, string nodeIdentifier, NodeEndpoint nodeEndpoint)
        {
            NodeType = nodeType;
            NodeIdentifier = nodeIdentifier;
            NodeEndpoint = nodeEndpoint;
        }

         
        

    }
}
