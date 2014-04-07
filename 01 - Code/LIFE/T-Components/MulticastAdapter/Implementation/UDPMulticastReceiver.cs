using System.Net;
using System.Net.Sockets;
using AppSettingsManager.Implementation;
using ConfigurationAdapter.Implementation;
using ConfigurationAdapter.Interface;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Config.Types;

namespace MulticastAdapter.Implementation
{
    public class UDPMulticastReceiver : IMulticastReciever
    {
        private readonly IPAddress mcastAddress;
        private readonly UdpClient recieverClient;
        private readonly int listenPort;
        private readonly Configuration<GeneralMulticastAdapterConfig> generalSettings;


        public UDPMulticastReceiver()
        {
            var path = "./" + typeof(UDPMulticastReceiver).Name;
            generalSettings = new Configuration<GeneralMulticastAdapterConfig>(path);


            mcastAddress = IPAddress.Parse(generalSettings.Content.MulticastGroupeIP);
            listenPort = generalSettings.Content.ListenPort;
            recieverClient = GetClient(listenPort);
            JoinMulticastGroupes();
        }


        public UDPMulticastReceiver(IPAddress mCastAdr, int listenPort)
            : this()
        {

            mcastAddress = mCastAdr;
            this.listenPort = listenPort;


        }


        private void JoinMulticastGroupes()
        {
            foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces())
            {
                foreach (var unicastAddr in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastAddr.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily())
                        recieverClient.JoinMulticastGroup(mcastAddress, unicastAddr.Address);
                }
            }
        }

        private UdpClient GetClient(int listenPort)
        {
            IPAddress listenAddress;

            switch (generalSettings.Content.IpVersion)
            {
                case IPVersionType.IPv6:
                    listenAddress = IPAddress.IPv6Any;
                    break;
                default:
                    listenAddress = IPAddress.Any;
                    break;
            }

            return new UdpClient(new IPEndPoint(listenAddress, listenPort));
        }


        /// <summary>
        ///     Listen to the multicastgrpup on the defient interface and waits for meesages. Returns the bytesteam the message as
        ///     soon as one message has arrived (blocking).
        /// </summary>
        /// <returns> the written bytestream</returns>
        public byte[] readMulticastGroupMessage()
        {
            IPEndPoint sourceEndPoint;
            byte[] msg = { };
            if (MulticastNetworkUtils.GetAddressFamily() == AddressFamily.InterNetworkV6)
                sourceEndPoint = new IPEndPoint(IPAddress.IPv6Any, listenPort);
            else sourceEndPoint = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                if (recieverClient.Client != null) msg = recieverClient.Receive(ref sourceEndPoint);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode != 10004) throw;
            }
            return msg;
        }

        public void CloseSocket()
        {
            recieverClient.Close();
        }
    }
}