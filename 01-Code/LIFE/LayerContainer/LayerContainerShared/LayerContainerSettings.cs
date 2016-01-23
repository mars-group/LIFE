// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using AppSettingsManager;
using CommonTypes.Types;
using MulticastAdapter.Interface.Config;
using NodeRegistry.Interface;

namespace LayerContainerShared {
    [Serializable]
    public class LayerContainerSettings {
        public NodeRegistryConfig NodeRegistryConfig { get; set; }

        public GlobalConfig GlobalConfig { get; set; }
        public MulticastSenderConfig MulticastSenderConfig { get; set; }

        public LayerContainerSettings() {
            string ipAddress = "127.0.0.1";
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces()) {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses) {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork) ipAddress = ip.Address.ToString();
                    }
                }
            }

            NodeRegistryConfig = new NodeRegistryConfig(NodeType.LayerContainer, "LC-1", ipAddress, 60100, true);
            GlobalConfig = new GlobalConfig();
            MulticastSenderConfig = new MulticastSenderConfig();
        }
    }
}