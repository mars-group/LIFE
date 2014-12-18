using System;

namespace LNSConnector.TransportTypes
{
    [Serializable]
    public class TLayerNameServiceEntry
    {
        public string IpAddress { get; private set; }
        public int Port { get; private set; }
        public Type LayerType { get; private set; }

        public TLayerNameServiceEntry(string ipAddress, int port, Type layerType)
        {
            IpAddress = ipAddress;
            Port = port;
            LayerType = layerType;
        }

        protected bool Equals(TLayerNameServiceEntry other)
        {
            return string.Equals(IpAddress, other.IpAddress) && Port == other.Port && Equals(LayerType, other.LayerType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TLayerNameServiceEntry) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (IpAddress != null ? IpAddress.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Port;
                hashCode = (hashCode*397) ^ (LayerType != null ? LayerType.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
