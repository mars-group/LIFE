using System;
using CommonModelTypes.Interface.SimObjects;

namespace CommonModelTypes.Interface.Datatypes
{
    public class Area
    {
        private Vector3D Position3D;
        private int Radius;


        public Area(Vector3D position3D, int radius)
        {
            Position3D = position3D;
            Radius = radius;
        }


        public Vector3D GetRnd2DPositinInsideArea()
        {
            //TODO brainen ob das so richtig ist
            var rnd = new Random();
            
            var newRndXPos = Position3D.X + rnd.Next(Radius * -1, Radius); ;
            var newRndYPos = Position3D.Y + rnd.Next(Radius * -1, Radius); ;

            return new Vector3D(newRndXPos, newRndYPos, 0);
        }

    }
}
