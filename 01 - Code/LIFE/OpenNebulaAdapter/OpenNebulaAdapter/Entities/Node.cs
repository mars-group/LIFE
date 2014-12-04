// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 06.11.2014
//  *******************************************************/

using System.Collections.Generic;

namespace OpenNebulaAdapter.Entities {
    public class Node {
        public string NodeName { get; private set; }
        public int CpuCount { get; private set; }
        public int RamAmount { get; private set; }
        public int PhysicalHost { get; private set; }

        public Node(IDictionary<string, dynamic> node) {
            NodeName = node["nodeName"];
            CpuCount = node["cpuCount"];
            RamAmount = node["ramAmount"];
            PhysicalHost = node["physicalHost"];
        }
    }
}