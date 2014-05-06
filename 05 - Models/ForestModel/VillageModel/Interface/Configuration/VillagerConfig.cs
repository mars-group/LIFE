using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageModel.Interface.Configuration
{
    class VillagerConfig
    {
        public int DailyWoodUsage;
        public int ChuckingAreaSize;
        public int treesPerPlan;

        //for alter use
        public int dmgPerSwing;

        public VillagerConfig(int dailyWoodUsage, int chuckingAreaSize)
        {
            DailyWoodUsage = dailyWoodUsage;
            ChuckingAreaSize = chuckingAreaSize;
            treesPerPlan = 4;

        }

        public VillagerConfig()
        {
            DailyWoodUsage = 100;
            ChuckingAreaSize = 10;
        }
    }
}
