using System.Net;
using CommonTypes.Types;

namespace CommonTypes.DataTypes
{
    /// <summary>
    /// The endpoint of a node participating in the LIFE system
    /// </summary>
    public class NodeEndpoint
    {
        public string IpAddress { get; private set; }

        public int Port { get; private set; }

        

        public NodeEndpoint(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
           
        }
    }
}