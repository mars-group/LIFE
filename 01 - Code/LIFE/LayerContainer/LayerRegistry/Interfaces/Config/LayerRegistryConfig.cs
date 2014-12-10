// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

namespace LayerRegistry.Interfaces.Config {
    public class LayerRegistryConfig {
        public readonly string MainNetworkAddress;
        public readonly int MainNetworkPort;

        public LayerRegistryConfig() {
            MainNetworkAddress = "10.0.0.7";
            MainNetworkPort = 8500;
        }


        public LayerRegistryConfig(string mainNetworkAddress, int mainNetworkPort) {
            MainNetworkAddress = mainNetworkAddress;
            MainNetworkPort = mainNetworkPort;
        }
    }
}