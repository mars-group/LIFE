using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MulticastAdapter.Interface;

namespace MulticastAdapter.Implementation
{
    class UDPMulticastReceiver : IMulticastReciever
    {

        private UdpClient recieverClient;
        private int sourcePort;


        public UDPMulticastReceiver(IPAddress mCastAdr, int sourcePort)
        {

            this.sourcePort = sourcePort;
            this.recieverClient = new UdpClient(AddressFamily.InterNetwork);
            recieverClient.JoinMulticastGroup(mCastAdr);

            BindSocketToNetworkinterface();
        }


        /// <summary>
        /// bind the udp socket to an interface
        /// </summary>
        private void BindSocketToNetworkinterface()
        {

            //Get all relveant Networkinterfaces(try to filter intrerface that dont enable mutlicast and virtual interfaces)
            var multicastInterfaces = MulticastNetworkUtils.GetAllMulticastInterfaces();

            //Binds the filter Interfaces to the UDP socket. 
            foreach (var multicastInterface in multicastInterfaces)
            {
                if (!recieverClient.Client.IsBound)
                {
                    recieverClient.Client.Bind(GetNetworkInterfaceEndpoint());
                }
            }
        }


        /// <summary>
        /// Returns on which endpoint the socket is listening to. This can be every networkinterface or a specific one. The behaviour can be configured in the appsettings.
        /// </summary>
        /// <exception cref="MissingArgumentException">This exception is thrown when u want to listen to a specific interface, but your description of the specific Interface is missing, or use a wrong key</exception>
        /// <exception cref="NoInterfaceFoundException">This exception is thrown when u want to listen to a specific interface and no interface with your given description was found. </exception>
        /// <returns>An IPEndPoint that contains the ip address of the Networkinterface (can be Any) and the port on which the socket is listening. Can NOT be null</returns>
        private IPEndPoint GetNetworkInterfaceEndpoint()
        {
            IPEndPoint sourceEndPoint = null;


            if (ConfigurationManager.AppSettings.AllKeys.Contains("GetListenInterfaceByName"))
            {
                var interfaceNames = ConfigurationManager.AppSettings.Get("GetListenInterfaceByName");
                var multicastInterface = MulticastNetworkUtils.GetInterfaceByName(interfaceNames);

                if (multicastInterface == null)
                {
                    throw new NoInterfaceFoundException("No interface with the given Name " + interfaceNames + " was found. Please check if your interface description, in app.config, is right.");
                }
                foreach (var unicastAddress in multicastInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastAddress.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily())
                    {
                        sourceEndPoint = new IPEndPoint(unicastAddress.Address, sourcePort);
                        break;
                    }
                }
            }
            else if (ConfigurationManager.AppSettings.AllKeys.Contains("GetListenInterfaceByIP"))
            {
                var ipAddress = IPAddress.Parse(ConfigurationManager.AppSettings.Get("GetListenInterfaceByIP"));
                var multicastInterface = MulticastNetworkUtils.GetInterfaceByIP(ipAddress);

                if (multicastInterface == null)
                {
                    throw new NoInterfaceFoundException("No interface with the given IP " + ipAddress + " was found. Please check if your interface description, in app.config, is right.");
                }

                foreach (var unicastAddress in multicastInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastAddress.Address.Equals(ipAddress))
                    {
                        sourceEndPoint = new IPEndPoint(ipAddress, sourcePort);
                        break; ;
                    }
                }
            }
            else
            {
                throw new MissingArgumentException("Missing argument in Configfile. if u dont want to listen to all networkinterfaces u have to define a network. Use Key = GetListenInterfaceByName  value = <Interfacename>, or Key = GetListenInterfaceByIP value = <IP>");
            }


            return sourceEndPoint;
        }





        /// <summary>
        /// Listen to the multicastgrpup on the defient interface and waits for meesages. Returns the bytesteam the message as soon as one message has arrived (blocking).
        /// </summary>
        /// <returns> the written bytestream</returns>
        public byte[] readMulticastGroupMessage()
        {

            IPEndPoint sourceEndPoint;

            if (MulticastNetworkUtils.GetAddressFamily() == AddressFamily.InterNetworkV6)
            {
                sourceEndPoint = new IPEndPoint(IPAddress.IPv6Any, sourcePort);
            }
            else
            {
                sourceEndPoint = new IPEndPoint(IPAddress.Any, sourcePort);
            }



            return recieverClient.Receive(ref sourceEndPoint);
        }

        public void CloseSocket()
        {
            recieverClient.Close();
        }

    }
}
