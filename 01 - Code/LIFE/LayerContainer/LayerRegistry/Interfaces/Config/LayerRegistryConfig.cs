namespace LayerRegistry.Interfaces.Config
{
    class LayerRegistryConfig {
        public readonly string MainNetworkAddress;
        public readonly int MainNetworkPort;

        public LayerRegistryConfig() {
            MainNetworkAddress = "10.0.0.7";
            MainNetworkPort = 8500;
        }


        public LayerRegistryConfig(string mainNetworkAddress, int mainNetworkPort) {
            MainNetworkAddress = mainNetworkAddress;
            MainNetworkPort = mainNetworkPort;
        }
    }
}
