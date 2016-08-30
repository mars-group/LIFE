//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 30.12.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using ConfigurationAdapter;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Config.Types;
using MulticastAdapter.Interface.Exceptions;

[assembly: InternalsVisibleTo("MulticastAdapterTest")]

namespace MulticastAdapter.Implementation {

    public class UDPMulticastSender : IMulticastSender {

        private readonly IPAddress _mGrpAdr;
        private readonly int _listenPort;
        private readonly GlobalConfig _generalSettings;
        private readonly MulticastSenderConfig _senderSettings;
        private IList<UdpClient> _clients;
        private int _sendingPort;


        public UDPMulticastSender(GlobalConfig generalConfig, MulticastSenderConfig senderConfig) {
            _generalSettings = generalConfig;
            _senderSettings = senderConfig;

            _mGrpAdr = IPAddress.Parse(_generalSettings.MulticastGroupIp);
            _sendingPort = _generalSettings.MulticastGroupSendingStartPort;
            _listenPort = _generalSettings.MulticastGroupListenPort;
            _clients = GetSendingInterfaces();
        }

        #region IMulticastSender Members

        public void CloseSocket() {
            foreach (UdpClient client in _clients) {

                client.Dispose();//.Client.Shutdown(SocketShutdown.Send);
            }
        }

        public void ReopenSocket() {
            foreach (var client in _clients) {
                client.Client.Shutdown(SocketShutdown.Send);
            }

            _clients = GetSendingInterfaces();
        }


        public void SendMessageToMulticastGroup(byte[] msg) {
			var errorCounter = 0;
			foreach (UdpClient client in _clients) {
                try {
                    if (client.Client != null) {
                        client.SendAsync(msg, msg.Length, new IPEndPoint(_mGrpAdr, _listenPort)).Wait();
                    }
                }
                catch (Exception ex) {
					if (_clients.Count == 1 || errorCounter >= _clients.Count) {
						// no interface was capable of sending, so stop and throw exception
						throw ex;
					} 
                }
            }
        }

        #endregion

        /// <summary>
        ///     Gets the sending interfaces.
        ///     Either only creates a updClient for the configured interface
        ///     or selects all multicast enabled interfaces and creates udpClients for them
        /// </summary>
        /// <returns>The sending interfaces. Empty if none.</returns>
        private IList<UdpClient> GetSendingInterfaces() {
            IList<UdpClient> resultList = new List<UdpClient>();


            if (_senderSettings.SendOnAllInterfaces) {
                foreach (NetworkInterface networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces()) {
                    foreach (
                        UnicastIPAddressInformation unicastAddress in
                            networkInterface.GetIPProperties().UnicastAddresses) {
                        if (unicastAddress.Address.AddressFamily
                            == MulticastNetworkUtils.GetAddressFamily((IPVersionType) _generalSettings.IPVersion)) {
                            UdpClient updClient = SetupSocket(unicastAddress.Address);
                            updClient.JoinMulticastGroup(_mGrpAdr, unicastAddress.Address);
                            resultList.Add(updClient);
                        }
                    }
                }
            }
            else {
                IPEndPoint endPoint = GetBindingEndpoint();
                UdpClient updClient = new UdpClient(GetBindingEndpoint());
                updClient.JoinMulticastGroup(_mGrpAdr, endPoint.Address);
                resultList.Add(updClient);
            }

            return resultList;
        }

        private UdpClient SetupSocket(IPAddress unicastAddress) {
            try {
                return new UdpClient(new IPEndPoint(unicastAddress, _sendingPort));
            }
            catch (SocketException socketException) {
                //if sending port is already in use increment port and try again.
                if (socketException.SocketErrorCode == SocketError.AddressAlreadyInUse) {
                    _sendingPort = _sendingPort + 1;
                    return SetupSocket(unicastAddress);
                }
                throw;
            }
        }


        private IPEndPoint GetBindingEndpoint() {
            switch (_senderSettings.BindingType) {
                case BindingType.IP:
                    return GetIPEndPointByIp();
                case BindingType.Name:
                    return GetIPEndPointByName();

                default:
                    // this should never happend
                    throw new NotImplementedException
                        (
                        "The type by which the binding interface is determined is not implemented.");
            }
        }

        private IPEndPoint GetIPEndPointByName() {
            NetworkInterface networkInterface =
                MulticastNetworkUtils.GetInterfaceByName(_senderSettings.SendingInterfaceName);
            IPAddress ipAddress = null;

            if (networkInterface == null) {
                throw new NoInterfaceFoundException
                    (
                    "No networkinterface with the IP " + _senderSettings.SendingInterfaceIP
                    + " was found. Please make sure that the IP is right and the interface up");
            }

            foreach (UnicastIPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses) {
                if (
                    unicastAddress.Address.AddressFamily.Equals
                        (
                            MulticastNetworkUtils.GetAddressFamily((IPVersionType) _generalSettings.IPVersion))) {
                    ipAddress = unicastAddress.Address;
                    break;
                }
            }

            if (ipAddress != null) {
                return new IPEndPoint(ipAddress, _sendingPort);
            }
            throw new NoInterfaceFoundException
                (
                "No interface with the given Name " + _senderSettings.SendingInterfaceName +
                " was found. Please check if your interface description is right and the Interface is up.");
        }

        private IPEndPoint GetIPEndPointByIp() {
            NetworkInterface networkInterface =
                MulticastNetworkUtils.GetInterfaceByIP(IPAddress.Parse(_senderSettings.SendingInterfaceIP));
            IPAddress ipAddress = null;

            if (networkInterface == null) {
                throw new NoInterfaceFoundException
                    (
                    "No networkinterface with the IP " + _senderSettings.SendingInterfaceIP
                    + " was found. Please make sure that the IP is right and the interface up");
            }

            foreach (UnicastIPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses) {
                if (unicastAddress.Address.AddressFamily.Equals
                    (
                        MulticastNetworkUtils.GetAddressFamily((IPVersionType) _generalSettings.IPVersion))) {
                    ipAddress = unicastAddress.Address;
                    break;
                }
            }

            if (ipAddress != null) {
                return new IPEndPoint(ipAddress, _sendingPort);
            }
            throw new NoInterfaceFoundException
                (
                "No interface with the given IP " + ipAddress +
                " was found. Please check if your interface description is right and the Interface is up.");
        }

        internal IList<UdpClient> GetSockets() {
            return _clients;
        }
    }

}