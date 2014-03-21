using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MulticastAdapter.Interface;

namespace MulticastAdapter.Implementation
{
    class MulticastNetworkUtils
    {


        /// <summary>
        /// Return the first networkinterface that matches the given name. If no Interface with the given name was found null is returned.
        /// </summary>
        /// <param name="name">name of the networkinterface</param>
        /// <returns></returns>
        public static NetworkInterface GetInterfaceByName(string name)
        {
            foreach(var networkInterface in GetAllMulticastInterfaces())
            {
                if (networkInterface.Name.Equals(name))
                {
                    return networkInterface;
                }

            }

            return null;
        }


        /// <summary>
        /// Returns the first networkinterface that matched the given IP-Address. If no Interface match the given address null will be returned.
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
                    if (unicastAddress.Address.Equals(ip))
                    {
                        return networkinterface;
                    }
                }
            }

            return null;

        }

        public static List<NetworkInterface> GetAllMulticastInterfaces()
        {
            return NetworkInterface.GetAllNetworkInterfaces().Where(
                networkInterface => networkInterface.SupportsMulticast &&
                                    networkInterface.GetIPProperties().MulticastAddresses.Any() &&
                                    OperationalStatus.Up == networkInterface.OperationalStatus
                                    ).ToList();
        }


        /// <summary>
        /// Parse which IP Version should be used from the Appsettings.
        /// </summary>
        /// <returns>The AddressFamily of the IPversion</returns>
        public static AddressFamily GetAddressFamily()
        {
            if (ConfigurationManager.AppSettings.Get("IpVersion").ToLower() == "ipv6")
            {
                return AddressFamily.InterNetworkV6;
            }
            else
            {
                return AddressFamily.InterNetwork;
            }
        }

        



    }
}
