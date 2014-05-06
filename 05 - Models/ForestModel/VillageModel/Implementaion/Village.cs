using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CommonModelTypes.Interface;
using CommonModelTypes.Interface.SimObjects;
using ForestModel.Interface;
using VillageModel.Interface.Configuration;

namespace VillageModel.Implementaion
{
    public class Village : SimObject
    {

        //TODO auf quardtree anpassen

        public RectangleF InfluenceRadius { get; private set; }
        private VillagerConfig _config;



        public Village(int id, RectangleF position, RectangleF influenceRadius)
            : base(id)
        {
            InfluenceRadius = influenceRadius;
        }


        public RectangleF GetRandomLocationInsideVillageRadius()
        {
            //TODO brainen
            var rnd = new Random();
            
            var minX = InfluenceRadius.X;
            var minY = InfluenceRadius.Y;
            var maxX = InfluenceRadius.Right;
            var maxY = InfluenceRadius.Top;
            
            return new RectangleF(rnd.Next((int) minX, (int) maxX), rnd.Next((int) minY, (int ) maxY), 1,1);
        }
    }
}
