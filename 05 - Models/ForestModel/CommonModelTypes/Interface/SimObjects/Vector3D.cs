using System;

namespace CommonModelTypes.Interface.SimObjects
{
    public class Vector3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3D(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
            this.Z = 0;
        }
        public Vector3D(double X, double Y, double Z)
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
