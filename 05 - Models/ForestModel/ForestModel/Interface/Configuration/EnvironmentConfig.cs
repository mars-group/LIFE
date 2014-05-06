using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestModel.Interface.Configuration
{
    class EnvironmentConfig
    {
        public int WorldWidth;
        public int WorldHeight;

        public int PatchWidth;
        public int PatchHeight;

        public EnvironmentConfig()
        {
            this.WorldWidth = 500;
            this.WorldHeight = 500;
            this.PatchWidth = 10;
            this.PatchHeight = 10;

        }
        
        public EnvironmentConfig(int worldWidth, int worldHeight, int patchWidth, int patchHeight)
        {
            this.WorldWidth = worldWidth;
            this.WorldHeight = worldHeight;
            this.PatchWidth = patchWidth;
            this.PatchHeight = patchHeight;
        }
    }
}
