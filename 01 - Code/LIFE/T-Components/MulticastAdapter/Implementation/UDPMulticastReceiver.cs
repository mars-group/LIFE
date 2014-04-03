using AppSettingsManager.Implementation;
using AppSettingsManager.Interface;
using MulticastAdapter.Interface;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;

namespace MulticastAdapter.Implementation
{
    public class UDPMulticastReceiver : IMulticastReciever
    {
        private IPAddress mcastAddress;
        private UdpClient recieverClient;
        private int listenPort;
        private IConfigurationAdapter configuration;

        public UDPMulticastReceiver()
        {
            this.configuration = new NiniAdapterImpl("MulticastAdapter");

            this.mcastAddress = configuration.GetIpAddress("IP");
            this.listenPort = configuration.GetInt32("ListenPort");
            recieverClient = GetClient(listenPort);
            JoinMulticastGroupes();
        }


        public UDPMulticastReceiver(IPAddress mCastAdr, int listenPort)
        {
            this.configuration = new NiniAdapterImpl("MulticastAdapter");
            this.mcastAddress = mCastAdr;
            this.listenPort = listenPort;
            recieverClient = GetClient(listenPort);
            JoinMulticastGroupes();
          
        }


        private void JoinMulticastGroupes()
        {
            foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces())
            {
                foreach (var unicastAddr in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastAddr.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily())
                    {
                        recieverClient.JoinMulticastGroup(mcastAddress, unicastAddr.Address);
                    }
                }
            }
        }

        private UdpClient GetClient(int listenPort)
        {

            if ("ipv6".Equals(configuration.GetValue("IpVersion").ToLower()))
            {
                return new UdpClient(new IPEndPoint(IPAddress.IPv6Any, listenPort));
            }
            else
            {
                return new UdpClient(new IPEndPoint(IPAddress.Any, listenPort)); 
            }
        }




        /// <summary>
        /// Listen to the multicastgrpup on the defient interface and waits for meesages. Returns the bytesteam the message as soon as one message has arrived (blocking).
        /// </summary>
        /// <returns> the written bytestream</returns>
        public byte[] readMulticastGroupMessage()
        {

            IPEndPoint sourceEndPoint;
            byte[] msg = new byte[] { };
            if (MulticastNetworkUtils.GetAddressFamily() == AddressFamily.InterNetworkV6)
            {
                sourceEndPoint = new IPEndPoint(IPAddress.IPv6Any, listenPort);
            }
            else
            {
                sourceEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
            }

            try
            {
                if (recieverClient.Client != null)
                {
                    msg = recieverClient.Receive(ref sourceEndPoint);
                }

            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode != 10004)
                {
                    throw;
                }

            }
            return msg;
        }

        public void CloseSocket()
        {
            recieverClient.Close();
        }

    }
}
