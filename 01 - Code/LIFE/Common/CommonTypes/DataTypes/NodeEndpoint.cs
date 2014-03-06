using System.Net;
using CommonTypes.Types;

namespace CommonTypes.DataTypes
{
    /// <summary>
    /// The endpoint of a node participating in the LIFE system
    /// </summary>
    public class NodeEndpoint
    {
        public IPAddress IpAddress { get; private set; }

        public int Port { get; private set; }

        public NodeType NodeType { get; private set; }

        public NodeEndpoint(IPAddress ipAddress, int port, NodeType nodeType)
        {
            IpAddress = ipAddress;
            Port = port;
            NodeType = nodeType;
        }
    }
}