using System;
using System.Net;
using System.Net.NetworkInformation;
using AppSettingsManager;
using LayerRegistry.Interfaces.Config;
using MulticastAdapter.Interface.Config;
using NodeRegistry.Interface;

namespace LayerContainerShared
{
    using CommonTypes.Types;

    [Serializable]
    public class LayerContainerSettings
    {
        public NodeRegistryConfig NodeRegistryConfig { get; set; }
        public LayerRegistryConfig LayerRegistryConfig { get; set; }

        public GlobalConfig GlobalConfig { get; set; }
        public MulticastSenderConfig MulticastSenderConfig { get; set; }

        public LayerContainerSettings() {

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

            NodeRegistryConfig = new NodeRegistryConfig(NodeType.LayerContainer, "LC-1", ipAddress, 60100, true);
            LayerRegistryConfig = new LayerRegistryConfig();
            GlobalConfig = new GlobalConfig();
            MulticastSenderConfig = new MulticastSenderConfig();
        }

    }
}
