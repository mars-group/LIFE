using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModelTypes.Interface;

namespace ForestModel.Implementation
{
    public abstract class SimObject
    {
        public int ID { get; private set; }
        public Vector3D Position { get; private set; }

        public SimObject(int id, Vector3D position) {
            ID = id;
            Position = position;
        }

    }
}
