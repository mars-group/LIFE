using System;

namespace LayerRegistry.Implementation
{
    internal class LayerRegistryEntry
    {


        public string IpAddress { get; private set; }
        public int Port { get; private set; }
        public Type LayerType { get; private set; }

        public LayerRegistryEntry(string ipAddress, int port, Type layerType)
        {
            IpAddress = ipAddress;
            Port = port;
            LayerType = layerType;
        }
    }
}
