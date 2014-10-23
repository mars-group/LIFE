using System;
using System.Net.NetworkInformation;
using MulticastAdapter.Interface.Config;
using NodeRegistry.Interface;

namespace SimulationManagerShared
{
    using CommonTypes.Types;

    /// <summary>
    /// This class holds all local settings for the SimulationManager.
    /// </summary>
    [Serializable]
    public class SimulationManagerSettings
    {
        public string AddinLibraryDirectoryPath { get; set; }
        public string AddinDirectoryPath { get; set; }
        public string ModelDirectoryPath { get; set; }
        public NodeRegistryConfig NodeRegistryConfig { get; set; }
        public MulticastSenderConfig MulticastSenderConfig { get; set; }

        public SimulationManagerSettings(string addinLibraryDirectoryPath, string modelDirectoryPath, NodeRegistryConfig nodeRegistryConfig, MulticastSenderConfig multicastSenderConfig) {
            AddinLibraryDirectoryPath = addinLibraryDirectoryPath;
            ModelDirectoryPath = modelDirectoryPath;
            NodeRegistryConfig = nodeRegistryConfig;
            MulticastSenderConfig = multicastSenderConfig;
        }

        //TODO: can this be internal?
        public SimulationManagerSettings() {
            var ipAddress = "127.0.0.1";
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            ipAddress = ip.Address.ToString();
                        }
                    }
                }
            }

            //AddinLibraryDirectoryPath = "./addinConfig";
            //ModelDirectoryPath = "./addinConfig/addins";
            AddinLibraryDirectoryPath = "./layers";
            ModelDirectoryPath = "./layers/addins";
            NodeRegistryConfig = new NodeRegistryConfig(NodeType.SimulationManager, "SM-1", ipAddress, 44521, true);
            MulticastSenderConfig = new MulticastSenderConfig();
        }
    }
}
