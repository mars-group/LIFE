
using System;
using System.CodeDom;
using System.Security.Cryptography;
using System.Text;
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


        public override string ToString()
        {
              var sb = new  StringBuilder();
                
              return sb.AppendFormat("[NodeIdentifier {0}, NodeNodeType {1}, NodeEndpoint {2}]", NodeIdentifier, NodeType, NodeEndpoint).ToString();


           
        }
       
        

    }
}
