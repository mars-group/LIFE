using System.Net;
using System.Net.Sockets;
using AppSettingsManager;
using ConfigurationAdapter.Interface;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Config.Types;

namespace MulticastAdapter.Implementation
{
    public class UDPMulticastReceiver : IMulticastReciever
    {
        private readonly IPAddress _mcastAddress;
        private UdpClient _recieverClient;
        private readonly int _listenPort;
        private readonly GlobalConfig _generalSettings;
        
        public UDPMulticastReceiver(IPAddress mCastAdr, int listenPort)
        {

            _mcastAddress = mCastAdr;
            this._listenPort = listenPort;
            JoinMulticastGroupes();

        }

        public UDPMulticastReceiver(GlobalConfig generalSeConfig) {
            _generalSettings = generalSeConfig;

            _mcastAddress = IPAddress.Parse(_generalSettings.MulticastGroupIp);
            _listenPort = _generalSettings.MulticastGroupListenPort;
            _recieverClient = GetClient(_listenPort);
            JoinMulticastGroupes();
        }


        private void JoinMulticastGroupes()
        {
            foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces())
            {
                foreach (var unicastAddr in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastAddr.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily((IPVersionType)_generalSettings.IPVersion))
                        _recieverClient.JoinMulticastGroup(_mcastAddress, unicastAddr.Address);
                }
            }
        }

        private UdpClient GetClient(int listenPort)
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
            udpClient.ExclusiveAddressUse = false;
            udpClient.Client.Bind(new IPEndPoint(listenAddress, listenPort));


            return udpClient;
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
            if (MulticastNetworkUtils.GetAddressFamily((IPVersionType) _generalSettings.IPVersion) == AddressFamily.InterNetworkV6)
                sourceEndPoint = new IPEndPoint(IPAddress.IPv6Any, _listenPort);
            else sourceEndPoint = new IPEndPoint(IPAddress.Any, _listenPort);

            try
            {
                if (_recieverClient.Client != null) msg = _recieverClient.Receive(ref sourceEndPoint);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode != 10004) throw;
            }
            return msg;
        }

        public void CloseSocket()
        {
            _recieverClient.Close();
        }

        public void ReopenSocket()
        {
            _recieverClient = GetClient(_listenPort);
        }
    }
}