﻿using System;
using NodeRegistry.Interface;

namespace Shared
{
    using AppSettingsManager;

    using LayerRegistry.Interfaces.Config;

    using MulticastAdapter.Interface.Config;



    [Serializable]
    public class LayerContainerSettings
    {
        public NodeRegistryConfig NodeRegistryConfig { get; set; }
        public LayerRegistryConfig LayerRegistryConfig { get; set; }

        public GlobalConfig GlobalConfig { get; set; }
        public MulticastSenderConfig MulticastSenderConfig { get; set; }

        public LayerContainerSettings() {
            NodeRegistryConfig = new NodeRegistryConfig();
            LayerRegistryConfig = new LayerRegistryConfig();
            GlobalConfig = new GlobalConfig();
            MulticastSenderConfig = new MulticastSenderConfig();
        }

    }
}
