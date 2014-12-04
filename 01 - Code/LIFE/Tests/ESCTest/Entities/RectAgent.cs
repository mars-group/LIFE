using System;
using EnvironmentServiceComponent.Entities.Shape;
using SpatialCommon.Enums;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;

namespace ESCTest.Entities {

    internal class RectAgent : ISpatialEntity {
        private readonly CollisionType _collisionType;

        public RectAgent(double x, double y, CollisionType collisionType = CollisionType.MassiveAgent) {
            _collisionType = collisionType;
            Shape = new RectShape(TVector.Origin, new TVector(x, y));
        }

        #region ISpatialEntity Members

        public IShape Shape { get; set; }

        public Enum GetCollisionType() {
            return _collisionType;
        }


        public Enum GetInformationType() {
            throw new NotImplementedException();
        }

        #endregion
    }

}