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
            validateMulticastGroup(globalConfiguration.MulticastGroupIp);

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

        private void validateMulticastGroup(string mcastIp) {


            if (IPAddress.Parse(mcastIp).IsIPv6Multicast || MulticastNetworkUtils.IsIPv4Multicast(mcastIp))
            {
             return; 
            }
            //TODO schauen wie ipv6 mcast ips ausschauen.
            throw new InvalidConfigurationException("The configured IP " + mcastIp + " is not a valid in this context. Use a IPv4 or IPv6 multicast IP.");
        }


    }
}
