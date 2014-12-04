// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

using System;

namespace LayerRegistry.Implementation {
    internal class LayerRegistryEntry {
        public string IpAddress { get; private set; }
        public int Port { get; private set; }
        public Type LayerType { get; private set; }

        public LayerRegistryEntry(string ipAddress, int port, Type layerType) {
            IpAddress = ipAddress;
            Port = port;
            LayerType = layerType;
        }
    }
}