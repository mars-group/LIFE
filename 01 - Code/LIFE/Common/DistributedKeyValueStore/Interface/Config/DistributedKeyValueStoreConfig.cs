// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

using System;

namespace DistributedKeyValueStore.Interface.Config {
    [Serializable]
    public class DistributedKeyValueStoreConfig {
        public int KademliaPort { get; set; }

        public DistributedKeyValueStoreConfig() {
            KademliaPort = 8500;
        }


        public DistributedKeyValueStoreConfig(int kademliaPort) {
            KademliaPort = kademliaPort;
        }
    }
}