using System.Net;
using CommonTypes.Types;
using ProtoBuf;

namespace CommonTypes.DataTypes
{
    /// <summary>
    /// The endpoint of a node participating in the LIFE system
    /// </summary>
    
    [ProtoContract]
    public class NodeEndpoint
    {
        [ProtoMember(1)]
        public string IpAddress { get; private set; }
        
        [ProtoMember(2)]
        public int Port { get; private set; }

        private NodeEndpoint()
        {
            
        }

        public NodeEndpoint(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port; 
        }
    }
}