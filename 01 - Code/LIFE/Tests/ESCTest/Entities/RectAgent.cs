using System;
using EnvironmentServiceComponent;
using EnvironmentServiceComponent.Entities.Shape;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;

namespace ESCTest.Entities {

    internal class RectAgent : ISpatialEntity {
        public RectAgent(double x, double y)
        {
            Shape = new RectShape(TVector.Origin, new TVector(x,y));
        }

        #region ISpatialEntity Members

        public Enum GetCollisionType() {
            return CollisionType.MassiveAgent;
        }


        public Enum GetInformationType() {
            throw new NotImplementedException();
        }

        public IShape Shape { get; set; }

        #endregion
    }

}