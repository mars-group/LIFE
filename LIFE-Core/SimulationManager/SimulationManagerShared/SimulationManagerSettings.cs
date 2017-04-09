//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MulticastAdapter.Interface.Config;
using NodeRegistry.Interface;
using CommonTypes.Types;
using NodeRegistry.Interface.Config;
using static System.String;


namespace SimulationManagerShared
{
    /// <summary>
    /// This class holds all local settings for the SimulationManager.
    /// </summary>
    [Serializable]
    public class SimulationManagerSettings
    {
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

        public SimulationManagerSettings()
        {
            var foundAddress = "";
            try
            {
                foundAddress = NetworkInterface.GetAllNetworkInterfaces()
                    .First(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    .GetIPProperties()
                    .UnicastAddresses
                    .First(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    .Address.ToString();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Address resolution failed, will use 127.0.0.1 instead");
                // ignored
            }


            var ipAddress = foundAddress != Empty ? foundAddress : "127.0.0.1";
            var fqdn = "";
            try
            {
                // Step 1: Get the host name
                var hostname = Dns.GetHostName();
                // Step 2: Perform a DNS lookup.
                // Note that the lookup is not guaranteed to succeed, especially
                // if the system is misconfigured. On the other hand, if that
                // happens, you probably can't connect to the host by name, anyway.
                var hostinfo = Dns.GetHostEntryAsync(hostname).Result;
                // Step 3: Retrieve the canonical name.
                fqdn = hostinfo.HostName;
            }
            catch (SocketException ex)
            {
                Console.Error.WriteLine(
                    "Dns Hostname Resolution failed, using random GUID as unique identifier for SimulationManager");
                fqdn = Guid.NewGuid().ToString();
            }

            var smName = "SM-" + fqdn;
            ModelDirectoryPath = "." + Path.DirectorySeparatorChar + "models";
            NodeRegistryConfig = new NodeRegistryConfig(NodeType.SimulationManager, smName, ipAddress, 44521, true);
            MulticastSenderConfig = new MulticastSenderConfig();
        }
    }
}