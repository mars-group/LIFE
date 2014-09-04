using System;
using System.Collections.Generic;
namespace OpenNebulaAdapter.Entities
{
    class Node {
        protected string NodeName { get; private set; }
        protected int CpuCount { get; private set; }
        protected int RamAmount { get; private set; }
        protected int PhysicalHost { get; private set; }

        public Node(IDictionary<string, dynamic> node) {
            NodeName = node["nodeName"];
            CpuCount = node["cpuAmount"];
            RamAmount = node["ramAmount"];
            PhysicalHost = node["physicalHost"];
        }


    }
}
