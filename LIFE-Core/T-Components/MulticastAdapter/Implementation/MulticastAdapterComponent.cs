//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using System.Net;
using System.Threading;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Exceptions;
using ConfigurationAdapter;

namespace MulticastAdapter.Implementation
{
    public class MulticastAdapterComponent : IMulticastAdapter
    {
        #region properties & fields

        private IMulticastSender _sender;
        private IMulticastReceiver _reciever;
        private Thread _listenThread;

        #endregion

        #region Constructors

        public MulticastAdapterComponent(GlobalConfig globalConfiguration, MulticastSenderConfig senderConfiguration)
        {
            ValidateMulticastGroup(globalConfiguration.MulticastGroupIp);
            _sender = new UDPMulticastSender(globalConfiguration, senderConfiguration);
            _reciever = new UDPMulticastReceiver(globalConfiguration);
        }

        #endregion

        public byte[] ReadMulticastGroupMessage()
        {
            return _reciever.ReadMulticastGroupMessage();
        }

        public void SendMessageToMulticastGroup(byte[] msg)
        {
            _sender.SendMessageToMulticastGroup(msg);
        }

        public void CloseSocket()
        {
            _sender.CloseSocket();
            _reciever.CloseSocket();
        }

        public void ReopenSocket()
        {
            _sender.ReopenSocket();
            _reciever.ReopenSocket();
        }

        private void ValidateMulticastGroup(string mcastIp)
        {
            if (IPAddress.Parse(mcastIp).IsIPv6Multicast || MulticastNetworkUtils.IsIPv4Multicast(mcastIp))
            {
                return;
            }
            //TODO schauen wie ipv6 mcast ips ausschauen.
            throw new InvalidConfigurationException("The configured IP " + mcastIp +
                                                    " is not a valid in this context. Use a IPv4 or IPv6 multicast IP.");
        }
    }
}