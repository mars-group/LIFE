using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModelTypes.Interface;
using CommonModelTypes.Interface.SimObjects;

namespace VillageModel.Implementaion
{
    public class Village : SimObject
    {
        public Village(int id, Vector3D position) : base(id, position) { }

        public Vector3D GetRandomLocationInsideVillageRadius()
        {
            throw new NotImplementedException();
        }
    }
}
