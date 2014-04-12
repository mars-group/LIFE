namespace AppSettingsManager
{
    using System;

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
            this.MulticastGroupIp = "244.10.99.1";
            this.MulticastGroupListenPort = 50100;
            this.MulticastGroupSendingStartPort = 50500;
            this.DHTPort = 8500;
            this.IPVersion = 4;
        }

        public GlobalConfig(string multicastGroupIp, int multicastGroupListenPort, int multicastGroupSendingStartPort, int ipVersion)
        {
            this.MulticastGroupIp = multicastGroupIp;
            this.MulticastGroupListenPort = multicastGroupListenPort;
            this.MulticastGroupSendingStartPort = multicastGroupSendingStartPort;
            this.DHTPort = 8500;
            this.IPVersion = ipVersion;
        }
    }
}
