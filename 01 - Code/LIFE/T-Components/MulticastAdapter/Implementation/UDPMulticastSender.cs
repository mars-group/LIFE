using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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
        private readonly int _sendingPort;
        private readonly int _listenPort;
        private readonly Configuration<GeneralMulticastAdapterConfig> _generalSettings;
        private readonly Configuration<MulticastSenderConfig> _senderSettings;
        


        public UDPMulticastSender()
        {
            var path = "./" + typeof(UDPMulticastSender).Name;
            _generalSettings = new Configuration<GeneralMulticastAdapterConfig>(path + "General.config");
            _senderSettings = new Configuration<MulticastSenderConfig>(path + "sender.config");

            _mGrpAdr = IPAddress.Parse(_generalSettings.Content.MulticastGroupeIP);
            _sendingPort = _generalSettings.Content.SendingPort;
            _listenPort = _generalSettings.Content.ListenPort;
            _clients = GetSendingInterfaces();
        }

        public UDPMulticastSender(IPAddress ipAddress, int sendingPort, int listenPort)
            : this()
        {
            var path = "./" + typeof(UDPMulticastSender).Name;
            _generalSettings = new Configuration<GeneralMulticastAdapterConfig>(path);
            _senderSettings = new Configuration<MulticastSenderConfig>(path);

            _mGrpAdr = ipAddress;
            this._sendingPort = sendingPort;
            this._listenPort = listenPort;

            _clients = GetSendingInterfaces();
        }

        public UDPMulticastSender(GeneralMulticastAdapterConfig generalConfig, MulticastSenderConfig senderConfig)
        {
            _generalSettings = new Configuration<GeneralMulticastAdapterConfig>(generalConfig);
            _senderSettings = new Configuration<MulticastSenderConfig>(senderConfig);

            _mGrpAdr = IPAddress.Parse(_generalSettings.Content.MulticastGroupeIP);
            _sendingPort = _generalSettings.Content.SendingPort;
            _listenPort = _generalSettings.Content.ListenPort;
            _clients = GetSendingInterfaces();

        }

        private IList<UdpClient> GetSendingInterfaces()
        {
            IList<UdpClient> resultList = new List<UdpClient>();

            if (_senderSettings.Content.SendOnAllInterfaces)
            {
                foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces())
                {
                    foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (unicastAddress.Address.AddressFamily ==
                            MulticastNetworkUtils.GetAddressFamily(_generalSettings.Content.IpVersion))
                        {
                            var updClient = new UdpClient(new IPEndPoint(unicastAddress.Address, _sendingPort));
                            updClient.JoinMulticastGroup(_mGrpAdr, unicastAddress.Address);
                            resultList.Add(updClient);
                        }
                    }
                }
            }
            else
            {
                var updClient = new UdpClient(GetBindingEndpoint());
                updClient.JoinMulticastGroup(_mGrpAdr, IPAddress.Parse(_senderSettings.Content.SendingInterfaceIP));
                resultList.Add(updClient);
            }


            return resultList;
        }


        private IPEndPoint GetBindingEndpoint()
        {
            switch (_senderSettings.Content.BindingType)
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
                MulticastNetworkUtils.GetInterfaceByIP(IPAddress.Parse(_senderSettings.Content.SendingInterfaceIP));
            IPAddress ipAddress = null;

            foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (unicastAddress.Address.AddressFamily.Equals(
                        MulticastNetworkUtils.GetAddressFamily(_generalSettings.Content.IpVersion)))
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
                try {
                    client.DropMulticastGroup(_mGrpAdr);
                    client.Client.Close();
                    client.Close();
                }
                catch (Exception e) {
                    logger.Warn("Error from Type " + e.GetType() + " by shutting down multicast send service. Message is: " + e.Message );
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