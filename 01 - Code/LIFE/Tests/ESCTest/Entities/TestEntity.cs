using System;
using CSharpQuadTree;
using LifeAPI.Spatial;
using SpatialCommon.Shape;
using SpatialCommon.Transformation;
using Direction = SpatialCommon.Transformation.Direction;

namespace ESCTest.Entities {

    public class TestEntity : ISpatialEntity {
        private readonly CollisionType _collisionType;

        public TestEntity(double x, double y, CollisionType collisionType = LifeAPI.Spatial.CollisionType.MassiveAgent) {
            _collisionType = collisionType;
//            Shape = new Cuboid(new Vector3(x, y), Vector3.Zero, new Direction());
            Shape = BoundingBox.GenerateByDimension(Vector3.Zero, new Vector3(x, y));
        }

        public IShape Shape { get; set; }
        public Enum CollisionType { get { return _collisionType; } }
        public Enum InformationType { get { return CollisionType; } }

        public override string ToString() {
            return Shape.Bounds.ToString();
        }
    }

}