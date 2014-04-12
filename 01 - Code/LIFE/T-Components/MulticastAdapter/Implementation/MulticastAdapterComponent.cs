using System;
using System.Net;
using System.Threading;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Exceptions;

namespace MulticastAdapter.Implementation
{
    using AppSettingsManager;

    public class MulticastAdapterComponent : IMulticastAdapter
    {

        #region properties & fields
        private IMulticastSender _sender;
        private IMulticastReciever _reciever;
        private Thread _listenThread;
        #endregion

        #region Constructors


        public MulticastAdapterComponent(GlobalConfig globalConfiguration, MulticastSenderConfig senderConfiguration)
        {
            _sender = new UDPMulticastSender(globalConfiguration, senderConfiguration);
            _reciever = new UDPMulticastReceiver(globalConfiguration);

        }


        #endregion

        public byte[] readMulticastGroupMessage()
        {
            return _reciever.readMulticastGroupMessage();
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

        private void validateMulticastGroup(GlobalConfig globalConfiguration)
        {

            if (IPAddress.Parse(globalConfiguration.MulticastGroupIp).IsIPv6Multicast || MulticastNetworkUtils.IsIPv4Multicast(globalConfiguration.MulticastGroupIp))
            {
             return; 
            }
            //TODO schauen wie ipv6 mcast ips ausschauen.
            throw new InvalidConfigurationException("The configured ip is not a valid IPv4 or IPv6 MulticastIP.");
        }


    }
}
