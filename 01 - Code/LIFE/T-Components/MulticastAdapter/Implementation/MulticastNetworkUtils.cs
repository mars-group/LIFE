using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using MulticastAdapter.Interface.Config.Types;

namespace MulticastAdapter.Implementation
{
    internal class MulticastNetworkUtils
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
                return NetworkInterface.GetAllNetworkInterfaces().Where
                    (
                        networkInterface => networkInterface.SupportsMulticast &&
					networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
					//networkInterface.GetIPProperties().MulticastAddresses.Any() &&
                                            OperationalStatus.Up == networkInterface.OperationalStatus
                    ).ToList();
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