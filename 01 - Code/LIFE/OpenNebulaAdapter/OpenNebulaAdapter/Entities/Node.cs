using System;
using System.Collections.Generic;
namespace OpenNebulaAdapter.Entities
{
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
