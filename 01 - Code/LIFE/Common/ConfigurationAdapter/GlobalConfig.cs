using System;
using System.Runtime.CompilerServices;
using ConfigurationAdapter.Interface;




namespace AppSettingsManager
{
    /// <summary>
    /// The global configuration for all LIFE processes.
    /// </summary>
    [Serializable]
    public class GlobalConfig
    {
        /// <summary>
        /// The IP of the multicast group, through which auto-discovery of nodes will happen.
        /// </summary>
        public string MulticastGroupIp { get; set; }

        /// <summary>
        /// The port of the multicast group
        /// </summary>
        public int MulticastGroupListenPort { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MulticastGroupSendingStartPort { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DHTPort { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int IPVersion { get; set; }

        public GlobalConfig()
        {
            MulticastGroupIp = "244.10.99.1";
            MulticastGroupListenPort = 50100;
            MulticastGroupSendingStartPort = 50500;
            DHTPort = 8500;
            IPVersion = 4;
        }

        public GlobalConfig(string multicastGroupIp, int multicastGroupListenPort, int multicastGroupSendingStartPort, int ipVersion)
        {
            MulticastGroupIp = multicastGroupIp;
            MulticastGroupListenPort = multicastGroupListenPort;
            MulticastGroupSendingStartPort = multicastGroupSendingStartPort;
            DHTPort = 8500;
            IPVersion = ipVersion;
        }
    }
}
