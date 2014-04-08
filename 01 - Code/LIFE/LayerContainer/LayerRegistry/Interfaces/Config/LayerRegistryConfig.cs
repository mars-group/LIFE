using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerRegistry.Interfaces.Config
{
    class LayerRegistryConfig {
        public string MainNetworkAddress;
        public int MainNetworkPort;
        public int KademliaPort;

        public LayerRegistryConfig() {
            MainNetworkAddress = "10.0.0.7";
            MainNetworkPort = 8500;
            KademliaPort = 8888;
        }


        public LayerRegistryConfig(string mainNetworkAddress, int mainNetworkPort, int kademliaPort) {
            MainNetworkAddress = mainNetworkAddress;
            MainNetworkPort = mainNetworkPort;
            KademliaPort = kademliaPort;
        }
    }
}
