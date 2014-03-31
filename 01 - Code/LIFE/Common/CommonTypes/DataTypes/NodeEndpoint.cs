using ProtoBuf;
using System.Text;

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

        public override string ToString()
        {
            return new StringBuilder().AppendFormat("{0} IP {1} Port {2}", this.GetType().Name, IpAddress, Port).ToString();
        }


    }
}