//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using ConfigurationAdapter;
using CommonTypes.Types;
using MulticastAdapter.Interface.Config;
using NodeRegistry.Interface;
using NodeRegistry.Interface.Config;
using static System.String;

namespace LayerContainerShared {
    [Serializable]
    public class LayerContainerSettings {
        public NodeRegistryConfig NodeRegistryConfig { get; set; }

        public GlobalConfig GlobalConfig { get; set; }
        public MulticastSenderConfig MulticastSenderConfig { get; set; }

        public LayerContainerSettings() {
            var foundAddress = "";
            try
            {
                foundAddress = NetworkInterface.GetAllNetworkInterfaces()
                    .First(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    .GetIPProperties().UnicastAddresses
                    .First(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork).Address.ToString();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Address resolution failes, will use 127.0.0.1 instead");
                // ignored
            }


            var ipAddress = foundAddress != Empty ? foundAddress : "127.0.0.1";
            var fqdn = "";
            try {
                // Step 1: Get the host name
                var hostname = Dns.GetHostName();
                // Step 2: Perform a DNS lookup.
                // Note that the lookup is not guaranteed to succeed, especially
                // if the system is misconfigured. On the other hand, if that
                // happens, you probably can't connect to the host by name, anyway.
                var hostinfo = Dns.GetHostEntryAsync(hostname).Result;
                // Step 3: Retrieve the canonical name.
                fqdn = hostinfo.HostName;
            }catch(SocketException ex){
                Console.Error.WriteLine("Dns Hostname Resolution failed, using random GUID as unique identifier for LayerContainer");
                fqdn = Guid.NewGuid().ToString();
            }

            var lcName = "LC-" + fqdn;
            NodeRegistryConfig = new NodeRegistryConfig(NodeType.LayerContainer, lcName, ipAddress, 60100, true);
            GlobalConfig = new GlobalConfig();
            MulticastSenderConfig = new MulticastSenderConfig();
        }
    }
}