using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModelTypes.Interface;
using CommonModelTypes.Interface.SimObjects;
using ForestModel.Interface;
using VillageModel.Interface.Configuration;

namespace VillageModel.Implementaion
{
    public class Village : SimObject
    {

        public RectangleF InfluenceRadius { get; private set; }
        private VillagerConfig _config;
        private ForestLayerContainer forestLayer;


        public Village(int id, RectangleF position, RectangleF influenceRadius)
            : base(id, position)
        {
            InfluenceRadius = influenceRadius;
        }


        public RectangleF GetRandomLocationInsideVillageRadius()
        {

            var rnd = new Random();
            
            var x = InfluenceRadius.X;
            var y = InfluenceRadius.Y;


            

        }
    }
}
