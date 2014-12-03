using System;
using EnvironmentServiceComponent.Entities.Shape;
using SpatialCommon.Enums;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;

namespace ESCTest.Entities {

    internal class RectAgent : ISpatialEntity {
        private readonly CollisionType _collisionType;

        public RectAgent(double x, double y, CollisionType collisionType) {
            _collisionType = collisionType;
            Shape = new RectShape(TVector.Origin, new TVector(x, y));
        }

        public RectAgent(double x, double y) : this(x, y, CollisionType.MassiveAgent) {}

        #region ISpatialEntity Members

        public Enum GetCollisionType() {
            return _collisionType;
        }


        public Enum GetInformationType() {
            throw new NotImplementedException();
        }

        public IShape Shape { get; set; }

        #endregion
    }

}