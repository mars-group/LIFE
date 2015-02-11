using System;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Movement;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Shape;

namespace ESCTest.Entities {

    public class TestEntity : ISpatialEntity {
        private readonly CollisionType _collisionType;

        public TestEntity
            (double x, double y, CollisionType collisionType = SpatialAPI.Entities.Movement.CollisionType.MassiveAgent) {
            _collisionType = collisionType;
            //            Shape = new Cuboid(new Vector3(x, y), Vector3.Zero, new Direction());
            Shape = BoundingBox.GenerateByDimension(Vector3.Zero, new Vector3(x, y));
        }

        public TestEntity
            (BoundingBox bounds, CollisionType collisionType = SpatialAPI.Entities.Movement.CollisionType.MassiveAgent) {
            _collisionType = collisionType;
            Shape = bounds;
        }

        public IShape Shape { get; set; }
        public Enum CollisionType { get { return _collisionType; } }
        public Guid AgentGuid { get; private set; }

        public override string ToString() {
            return Shape.Bounds.ToString();
        }
    }

}