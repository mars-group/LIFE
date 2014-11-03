using DalskiAgent.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PedestrianModel.Util.Math
{
    class Vector3DHelper
    {
        public static double Distance(Vector3D from, Vector3D to)
        {
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double dz = to.Z - from.Z;
            return System.Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static Vector ToDalskiVector(Vector3D vector3d)
        {
            return new Vector((float)vector3d.X, (float)vector3d.Y, (float)vector3d.Z);
        }

        public static Vector3D FromDalskiVector(Vector vector)
        {
            return new Vector3D(vector.X, vector.Y, vector.Z);
        }

    }
}
