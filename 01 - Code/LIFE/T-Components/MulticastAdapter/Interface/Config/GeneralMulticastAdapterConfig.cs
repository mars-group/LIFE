using System;
using MulticastAdapter.Interface.Config.Types;

namespace MulticastAdapter.Interface.Config
{
    public class GeneralMulticastAdapterConfig
    {
        public String MulticastGroupeIP;
        public int ListenPort;
        public int SendingPort;
        public IPVersionType IpVersion;

        public GeneralMulticastAdapterConfig(string multicastGroupeIp, int listenPort, int sendingPort, IPVersionType ipVersion)
        {
            MulticastGroupeIP = multicastGroupeIp;
            ListenPort = listenPort;
            SendingPort = sendingPort;
            IpVersion = ipVersion;
        }

           public GeneralMulticastAdapterConfig()
        {
            MulticastGroupeIP = "224.10.99.1";
            ListenPort = 50100;
            SendingPort = 50500;
            IpVersion = IPVersionType.IPv4;
        }

    }
}

