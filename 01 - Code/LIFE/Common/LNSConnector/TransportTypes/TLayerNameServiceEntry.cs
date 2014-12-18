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
    }
}
