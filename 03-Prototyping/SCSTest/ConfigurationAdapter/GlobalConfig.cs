﻿// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

using System;

namespace AppSettingsManager {
    /// <summary>
    ///     The global configuration for all LIFE processes.
    /// </summary>
    [Serializable]
    public class GlobalConfig {
        /// <summary>
        ///     The IP of the multicast group, through which auto-discovery of nodes will happen.
        /// </summary>
        public string MulticastGroupIp { get; set; }

        /// <summary>
        ///     The port of the multicast group
        /// </summary>
        public int MulticastGroupListenPort { get; set; }

        /// <summary>
        /// </summary>
        public int MulticastGroupSendingStartPort { get; set; }

        /// <summary>
        /// </summary>
        public int DHTPort { get; set; }

        /// <summary>
        /// </summary>
        public int IPVersion { get; set; }

        public GlobalConfig() {
            MulticastGroupIp = "224.10.99.1";
            MulticastGroupListenPort = 50100;
            MulticastGroupSendingStartPort = 50500;
            DHTPort = 8500;
            IPVersion = 4;
        }

        public GlobalConfig
            (string multicastGroupIp, int multicastGroupListenPort, int multicastGroupSendingStartPort, int ipVersion) {
            MulticastGroupIp = multicastGroupIp;
            MulticastGroupListenPort = multicastGroupListenPort;
            MulticastGroupSendingStartPort = multicastGroupSendingStartPort;
            DHTPort = 8500;
            IPVersion = ipVersion;
        }
    }
}