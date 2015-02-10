using System;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Movement;
using SpatialAPI.Shape;

namespace OctreeTest.Entity {

    public class TestEntity : ISpatialEntity {
        private readonly CollisionType _collisionType;

        public TestEntity
            (BoundingBox bounds, CollisionType collisionType = SpatialAPI.Entities.Movement.CollisionType.MassiveAgent) {
            _collisionType = collisionType;
            Shape = bounds;
        }

        public BoundingBox Bounds {
            get { return Shape.Bounds; } 
        }

        public IShape Shape { get; set; }
        public Enum CollisionType { get { return _collisionType; } }
        public Guid AgentGuid { get; private set; }

        public override string ToString() {
            return Shape.Bounds.ToString();
        }
    }

}