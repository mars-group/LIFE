using System;
using System.Text;
using CommonTypes.Types;
using ProtoBuf;

namespace CommonTypes.DataTypes
{
    [ProtoContract]
    [Serializable]
    public class NodeInformationType : IComparable
    {
        [ProtoMember(1)]
        public NodeType NodeType { get; private set; }

        [ProtoMember(2)]
        public string NodeIdentifier { get; private set; }

        [ProtoMember(3)]
        public NodeEndpoint NodeEndpoint { get; private set; }


        private NodeInformationType() { }

        public NodeInformationType(NodeType nodeType, string nodeIdentifier, NodeEndpoint nodeEndpoint)
        {
            NodeType = nodeType;
            NodeIdentifier = nodeIdentifier;
            NodeEndpoint = nodeEndpoint;
        }


        public override string ToString()
        {
            var sb = new StringBuilder();

            return
                sb.AppendFormat("[NodeIdentifier{0}, TNode{1}, NodeEndpoint{2}]", NodeIdentifier, NodeType, NodeEndpoint)
                    .ToString();
        }


        public override bool Equals(object obj)
        {
            var type = obj as NodeInformationType;
            if (type != null)
            {
                var otherNodeInfo = type;
                return (otherNodeInfo.NodeIdentifier.Equals(NodeIdentifier) && otherNodeInfo.NodeType.Equals(NodeType));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return NodeIdentifier.GetHashCode() * NodeType.GetHashCode() * 347;
        }

        public int CompareTo(object obj)
        {
            return NodeIdentifier.CompareTo((obj as NodeInformationType).NodeIdentifier);
        }
    }
}