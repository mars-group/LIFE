//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 25.01.2016
//  *******************************************************/
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using ConfigurationAdapter;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Config.Types;

[assembly: InternalsVisibleTo("MulticastAdapterTest")]

namespace MulticastAdapter.Implementation
{
    public class UDPMulticastReceiver : IMulticastReceiver
    {
        private readonly IPAddress _mcastAddress;
		private UdpClient _receiverClient;
        private int _listenPort;
        private readonly GlobalConfig _generalSettings;
        
        public UDPMulticastReceiver(IPAddress mCastAdr, int listenPort, int ipVersion = 4)
        {
            _generalSettings = new GlobalConfig(mCastAdr.ToString(), _listenPort, 0, ipVersion);
            _mcastAddress = mCastAdr;
            _listenPort = listenPort;
			_receiverClient = GetClient();
            JoinMulticastGroups();
        }

        public UDPMulticastReceiver(GlobalConfig generalSeConfig) {
            _generalSettings = generalSeConfig;

            _mcastAddress = IPAddress.Parse(_generalSettings.MulticastGroupIp);
            _listenPort = _generalSettings.MulticastGroupListenPort;
			_receiverClient = GetClient ();
            JoinMulticastGroups();
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

            // allow another client to bind to this port
            //udpClient.Client.MulticastLoopback = true;
            //udpClient.Client.ExclusiveAddressUse = false;

            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(new IPEndPoint(listenAddress, _listenPort));
            return udpClient;
        }

        private void JoinMulticastGroups()
        {
			foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces()) {
				foreach (var unicastAddr in networkInterface.GetIPProperties().UnicastAddresses) {
					if (unicastAddr.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily ((IPVersionType)_generalSettings.IPVersion)) {
                        _receiverClient.JoinMulticastGroup (_mcastAddress, unicastAddr.Address);
					}
				}

			}
        }

        /// <summary>
		///     Listens to the multicastgroup on the defined interface and waits for messages. Returns the bytestream for the message as
        ///     soon as one message has arrived (blocking).
        /// </summary>
        /// <returns> the written bytestream</returns>
        public byte[] ReadMulticastGroupMessage()
        {
            byte[] msg = { };

			while (msg.Length <= 0) {
				try
				{
				    if (_receiverClient.Client != null)
				    {
				        var recTask = _receiverClient.ReceiveAsync();
				        msg = recTask.Result.Buffer;
				    }
				}
				catch(ObjectDisposedException expo){
					_receiverClient = GetClient ();
					JoinMulticastGroups ();
				}
				catch (SocketException ex)
				{
					_receiverClient.Client.Dispose();
					if (ex.SocketErrorCode != SocketError.Interrupted && ex.SocketErrorCode != SocketError.TimedOut) throw;
				}
			}

            return msg;
        }

        public void CloseSocket()
        {
			_receiverClient.Client.Dispose();//.Client.Shutdown(SocketShutdown.Receive);
        }

        public void ReopenSocket()
        {
			_receiverClient = GetClient();
        }
    }
}