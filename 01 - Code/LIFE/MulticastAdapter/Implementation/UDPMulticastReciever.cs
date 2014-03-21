using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MulticastAdapter.Interface;

namespace MulticastAdapter.Implementation
{
    class UDPMulticastReciever : IMulticastReciever
    {

        private UdpClient recieverClient;
        private int sourcePort;


        public UDPMulticastReciever(IPAddress mCastAdr, int sourcePort)
        {

            this.sourcePort = sourcePort;
            this.recieverClient = new UdpClient(AddressFamily.InterNetwork);
            recieverClient.JoinMulticastGroup(mCastAdr);

            BindSocketToNetworkinterface();
        }

        private void BindSocketToNetworkinterface()
        {

            //Get all relveant Networkinterfaces(try to filter intrerface that dont enable mutlicast and virtual interfaces)
            var multicastInterfaces = NetworkInterface.GetAllNetworkInterfaces().Where(
                networkInterface => networkInterface.SupportsMulticast &&
                                    networkInterface.GetIPProperties().MulticastAddresses.Any() &&
                                    OperationalStatus.Up == networkInterface.OperationalStatus
                                    ).ToList();

            //Binds the filter Interfaces to the UDP socket. 
            foreach (var multicastInterface in multicastInterfaces)
            {
                if (!recieverClient.Client.IsBound)
                {
                    foreach (IPAddressInformation unicastAddress in multicastInterface.GetIPProperties().UnicastAddresses)
                    {
                        //TODO aus konfig lesen ob ip v4 oder v6 Multicast
                        //TODO prüfen ob man ipv4 multicastadresse in ipv6 adresse umwandeln kann

                        // Check which IP Version is enabled TODO atm only ipv4 is supported
                        if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetworkV6) continue;
                        var endPoint = new IPEndPoint(unicastAddress.Address, sourcePort);

                        recieverClient.Client.Bind(endPoint);
                        break;
                    }
                }
            }
        }


        public byte[] readMulticastGroupMessage()
        {

            

            IPEndPoint sourceEndPoint = new IPEndPoint(IPAddress.Any, sourcePort);
            return recieverClient.Receive(ref sourceEndPoint);

        }

        public void CloseSocket()
        {
            recieverClient.Close();
        }

    }
}
