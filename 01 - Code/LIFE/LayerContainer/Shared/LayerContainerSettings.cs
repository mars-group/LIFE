using System;
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
            NodeRegistryConfig = new NodeRegistryConfig(NodeType.LayerContainer, "LC-1", "127.0.0.1", 60100, true);
            LayerRegistryConfig = new LayerRegistryConfig();
            GlobalConfig = new GlobalConfig();
            MulticastSenderConfig = new MulticastSenderConfig();
        }

    }
}
