using System;

namespace CommonModelTypes.Interface.SimObjects
{
    public class Vector3D
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Vector3D(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            this.Z = 0;
        }
        public Vector3D(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public float DistanceToWayPoint(Vector3D ownPostion)
        {
            //TODO distance berechnen
            throw new NotImplementedException();
        }




    }
}
