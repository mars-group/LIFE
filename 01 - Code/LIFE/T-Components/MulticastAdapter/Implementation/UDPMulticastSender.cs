using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using AppSettingsManager;
using ConfigurationAdapter.Interface;
using ConfigurationAdapter.Interface.Exceptions;
using log4net;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Config.Types;
using MulticastAdapter.Interface.Exceptions;

namespace MulticastAdapter.Implementation
{
    public class UDPMulticastSender : IMulticastSender
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(UDPMulticastSender));

        private IList<UdpClient> _clients;
        private readonly IPAddress _mGrpAdr;
        private int _sendingPort;
        private readonly int _listenPort;
        private readonly Configuration<GlobalConfig> _generalSettings;
        private readonly Configuration<MulticastSenderConfig> _senderSettings;

   
        public UDPMulticastSender(Configuration<GlobalConfig> generalConfig, Configuration<MulticastSenderConfig> senderConfig)
        {
            _generalSettings = generalConfig;
            _senderSettings = senderConfig;

            _mGrpAdr = IPAddress.Parse(_generalSettings.Instance.MulticastGroupIp);
            _sendingPort = _generalSettings.Instance.MulticastGroupSendingStartPort;
            _listenPort = _generalSettings.Instance.MulticastGroupListenPort;
            _clients = GetSendingInterfaces();

        }

        private IList<UdpClient> GetSendingInterfaces()
        {
            IList<UdpClient> resultList = new List<UdpClient>();

            if (_senderSettings.Instance.SendOnAllInterfaces)
            {
                foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces())
                {
                    foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (unicastAddress.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily( (IPVersionType)_generalSettings.Instance.IPVersion ))
                        {
                            var updClient = SetupSocket(unicastAddress);
                            updClient.JoinMulticastGroup(_mGrpAdr, unicastAddress.Address);
                            resultList.Add(updClient);
                        }
                    }
                }
            }
            else
            {
                var updClient = new UdpClient(GetBindingEndpoint());
                updClient.JoinMulticastGroup(_mGrpAdr, IPAddress.Parse(_senderSettings.Instance.SendingInterfaceIP));
                resultList.Add(updClient);
            }


            return resultList;
        }

        private UdpClient SetupSocket(UnicastIPAddressInformation unicastAddress)
        {
            try
            {
                var updClient = SetupSocket(unicastAddress);
                return updClient;
            }
            catch (SocketException socketException)
            {
                //if sending port is already in use increment port and try again.
                if (socketException.ErrorCode == 10048)
                {
                    _sendingPort = _sendingPort + 1;
                    return SetupSocket(unicastAddress);
                }
                throw;
            }




        }




        private IPEndPoint GetBindingEndpoint()
        {
            switch (_senderSettings.Instance.BindingType)
            {
                case BindingType.IP:
                    return GetIPEndPointByIp();
                case BindingType.Name:
                    //TODO 
                    throw new NotImplementedException();
                default:
                    // this shut never happend
                    throw new NotImplementedException(
                        "The type by which the binding interface is determined is not implemented.");
            }
        }

        private IPEndPoint GetIPEndPointByIp()
        {
            var networkInterface =
                MulticastNetworkUtils.GetInterfaceByIP(IPAddress.Parse(_senderSettings.Instance.SendingInterfaceIP));
            IPAddress ipAddress = null;

            foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (unicastAddress.Address.AddressFamily.Equals(
                        MulticastNetworkUtils.GetAddressFamily((IPVersionType) _generalSettings.Instance.IPVersion)))
                {
                    ipAddress = unicastAddress.Address;
                    break;
                }
            }

            if (ipAddress != null) return new IPEndPoint(ipAddress, _sendingPort);
            throw new NoInterfaceFoundException("No interface with the given IP " + ipAddress +
                                                " was found. Please check if your interface description, in app.config, is right.");
        }

        public void CloseSocket()
        {
            foreach (var client in _clients)
            {
                try
                {
                    client.DropMulticastGroup(_mGrpAdr);
                    client.Client.Close();
                    client.Close();
                }
                catch (Exception e)
                {
                    logger.Warn("Error from Type " + e.GetType() + " by shutting down multicast send service. Message is: " + e.Message);
                }

            }
        }

        public void ReopenSocket()
        {
            CloseSocket();
            _clients = GetSendingInterfaces();
        }


        public void SendMessageToMulticastGroup(byte[] msg)
        {
            foreach (var client in _clients)
            {
                if (client.Client != null) client.Send(msg, msg.Length, new IPEndPoint(_mGrpAdr, _listenPort));
            }
        }
    }
}