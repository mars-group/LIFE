using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestModel.Interface.Configuration
{
    class TreeAgentConfiguration {
        public float MaxHeight;
        public float MaxDiameter;
        public float GrowthCoefficient;

        public TreeAgentConfiguration() {
            MaxHeight = 30.0f;
            MaxDiameter = 325f/3.14f;
            GrowthCoefficient =  0.18f;
        }
    }
}
