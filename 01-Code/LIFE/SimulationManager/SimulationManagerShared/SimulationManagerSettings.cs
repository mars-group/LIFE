//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
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
        /// <summary>
        /// The directory path for the addin library
        /// </summary>
        public string AddinLibraryDirectoryPath { get; set; }
        /// <summary>
        /// The addin directory path
        /// </summary>
        public string AddinDirectoryPath { get; set; }
        /// <summary>
        /// The model directory path
        /// </summary>
        public string ModelDirectoryPath { get; set; }
        /// <summary>
        /// The NoderegistryConfig for this SimulationManager
        /// </summary>
        public NodeRegistryConfig NodeRegistryConfig { get; set; }
        /// <summary>
        /// The MulticastSenderConfig for this SimulationManager
        /// </summary>
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
