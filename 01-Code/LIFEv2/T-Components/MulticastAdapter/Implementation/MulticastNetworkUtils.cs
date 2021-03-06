﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MulticastAdapter.Interface.Config.Types;

namespace MulticastAdapter.Implementation
{
    public class MulticastNetworkUtils
    {
        //TODO implement iterateOverAllNetworkInterfaces(delegate)

        /// <summary>
        ///     Return the first networkinterface that matches the given name. If no Interface with the given name was found null
        ///     is returned.
        /// </summary>
        /// <param name="name">name of the networkinterface</param>
        /// <returns></returns>
        public static NetworkInterface GetInterfaceByName(string name)
        {
            foreach (var networkInterface in GetAllMulticastInterfaces())
            {
                if (networkInterface.Name.Equals(name)) return networkInterface;
            }

            return null;
        }


        /// <summary>
        ///     Returns the first networkinterface that matched the given IP-Address. If no Interface match the given address null
        ///     will be returned.
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static NetworkInterface GetInterfaceByIP(IPAddress ip)
        {
            var Networkinterfaces = GetAllMulticastInterfaces();

            foreach (var networkinterface in Networkinterfaces)
            {
                foreach (var unicastAddress in networkinterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastAddress.Address.Equals(ip)) return networkinterface;
                }
            }

            return null;
        }

        public static List<NetworkInterface> GetAllMulticastInterfaces() {
                var result = NetworkInterface.GetAllNetworkInterfaces().Where
                    (
                        networkInterface => networkInterface.SupportsMulticast &&
						(networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)  && 
						OperationalStatus.Up == networkInterface.OperationalStatus && 
						networkInterface.GetIPProperties().UnicastAddresses.Any()
                    ).ToList();

            if (result.Count <= 0) {
                result = NetworkInterface.GetAllNetworkInterfaces().ToList();
            }

            return result;
        }


        /// <summary>
        ///     Parse which IP Version should be used from the Appsettings.
        /// </summary>
        /// <returns>The AddressFamily of the IPversion</returns>
        public static AddressFamily GetAddressFamily(IPVersionType ipVersion)
        {
            switch (ipVersion)
            {
                case IPVersionType.IPv6:
                    return AddressFamily.InterNetworkV6;
                default:
                    return AddressFamily.InterNetwork;
            }

        }

        public static bool IsIPv4Multicast(String ip)
        {
                var octet1 = Int32.Parse(ip.Split(new Char[] { '.' }, 4)[0]);
                if ((octet1 >= 224) && (octet1 <= 239))
                    return true;
            
            
            return false;
        }

    }
}