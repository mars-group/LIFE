
using CommonTypes.Types;

namespace CommonTypes.DataTypes
{
    public class NodeInformationType
    {
        public NodeType NodeType { get; private set; }
        public string NodeIdentifier { get; private set; }
        public NodeEndpoint NodeEndpoint { get; private set; }

        public NodeInformationType(NodeType nodeType, string nodeIdentifier, NodeEndpoint nodeEndpoint)
        {
            NodeType = nodeType;
            NodeIdentifier = nodeIdentifier;
            NodeEndpoint = nodeEndpoint;
        }
    }
}
