using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModelTypes.Interface;
using CommonModelTypes.Interface.Datatypes;
using CommonModelTypes.Interface.SimObjects;
using VillageModel.Interface.Configuration;

namespace VillageModel.Implementaion
{
    public class Village : SimObject
    {

        public Area InfluenceRadius { get; private set; }
        private VillagerConfig _config;


        public Village(int id, Vector3D position, Area influenceRadius) : base(id, position)
        {
            InfluenceRadius = influenceRadius;
        }


        public Area GetRandomLocationInsideVillageRadius()
        {
            return new Area(InfluenceRadius.GetRnd2DPositinInsideArea(), _config.ChuckingAreaSize);
        }
    }
}
