using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageModel.Interface.Configuration
{
    class VillagerConfig {
        public int DailyWoodUsage;
        public float WayPointReachedDistance;

        public VillagerConfig() {
            WayPointReachedDistance = 2.5f;
            DailyWoodUsage = 100;
        }
    }
}
