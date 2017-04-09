//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using System.Collections.Generic;

namespace ConfigurationAdapter
{
    /// <summary>
    ///     The global configuration for all LIFE processes.
    /// </summary>
    [Serializable]
    public class GlobalConfig
    {
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

        public List<string> Strings { get; set; }

        /// <summary>
        /// </summary>
        public int IPVersion { get; set; }

        public GlobalConfig()
        {
            MulticastGroupIp = "239.0.0.1";
            MulticastGroupListenPort = 50100;
            MulticastGroupSendingStartPort = 50500;
            IPVersion = 4;
        }

        public GlobalConfig
            (string multicastGroupIp, int multicastGroupListenPort, int multicastGroupSendingStartPort, int ipVersion)
        {
            MulticastGroupIp = multicastGroupIp;
            MulticastGroupListenPort = multicastGroupListenPort;
            MulticastGroupSendingStartPort = multicastGroupSendingStartPort;
            IPVersion = ipVersion;
        }
    }
}