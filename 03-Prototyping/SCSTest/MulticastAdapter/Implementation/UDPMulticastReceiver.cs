using System.Net;
using System.Net.Sockets;
using AppSettingsManager;
using ConfigurationAdapter.Interface;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Config.Types;

namespace MulticastAdapter.Implementation
{
    public class UDPMulticastReceiver : IMulticastReceiver
    {
        private readonly IPAddress _mcastAddress;
		private UdpClient _receiverClient;
        private readonly int _listenPort;
        private readonly GlobalConfig _generalSettings;
        
        public UDPMulticastReceiver(IPAddress mCastAdr, int listenPort, int ipVersion = 4)
        {
            _generalSettings = new GlobalConfig(mCastAdr.ToString(), _listenPort, 0, ipVersion);
            _mcastAddress = mCastAdr;
            _listenPort = listenPort;
			_receiverClient = GetClient();
            JoinMulticastGroupes();

        }

        public UDPMulticastReceiver(GlobalConfig generalSeConfig) {
            _generalSettings = generalSeConfig;

            _mcastAddress = IPAddress.Parse(_generalSettings.MulticastGroupIp);
            _listenPort = _generalSettings.MulticastGroupListenPort;
			_receiverClient = GetClient();
            JoinMulticastGroupes();
        }


        private void JoinMulticastGroupes()
        {
            foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces())
            {
                foreach (var unicastAddr in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastAddr.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily((IPVersionType)_generalSettings.IPVersion))
						_receiverClient.JoinMulticastGroup(_mcastAddress, unicastAddr.Address);
                }
            }
        }

        private UdpClient GetClient()
        {

            IPAddress listenAddress;

            switch ((IPVersionType)_generalSettings.IPVersion)
            {
                case IPVersionType.IPv6:
                    listenAddress = IPAddress.IPv6Any;
                    break;
                default:
                    listenAddress = IPAddress.Any;
                    break;
            }

            var udpClient = new UdpClient();

            udpClient.ExclusiveAddressUse = false;
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(new IPEndPoint(listenAddress, _listenPort));

            return udpClient;

        }


        /// <summary>
		///     Listens to the multicastgroup on the defined interface and waits for messages. Returns the bytestream for the message as
        ///     soon as one message has arrived (blocking).
        /// </summary>
        /// <returns> the written bytestream</returns>
        public byte[] readMulticastGroupMessage()
        {
            IPEndPoint sourceEndPoint;
            byte[] msg = { };

			// check for IP Version and initialize sourceEndPoint accordingly
			if (MulticastNetworkUtils.GetAddressFamily ((IPVersionType)_generalSettings.IPVersion) == AddressFamily.InterNetworkV6) {
				sourceEndPoint = new IPEndPoint (IPAddress.IPv6Any, 0);
			} else {
				sourceEndPoint = new IPEndPoint (IPAddress.Any, 0);

			}

            try
            {
				if (_receiverClient.Client != null) msg = _receiverClient.Receive(ref sourceEndPoint);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode != 10004) throw;
            }
            return msg;
        }

        public void CloseSocket()
        {
			_receiverClient.Close();
        }

        public void ReopenSocket()
        {
			_receiverClient = GetClient();
        }
    }
}