// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

namespace DistributedKeyValueStore.Interface.Config {
    public class DistributedKeyValueStoreConfig {
        public readonly int KademliaPort;

        public DistributedKeyValueStoreConfig() {
            KademliaPort = 8500;
        }


        public DistributedKeyValueStoreConfig(int kademliaPort) {
            KademliaPort = kademliaPort;
        }
    }
}