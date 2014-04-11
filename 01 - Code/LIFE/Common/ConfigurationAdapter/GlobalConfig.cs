using System;

namespace AppSettingsManager
{
    /// <summary>
    /// The global configuration for all LIFE processes.
    /// </summary>
    [Serializable]
    class GlobalConfig
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
    }
}
