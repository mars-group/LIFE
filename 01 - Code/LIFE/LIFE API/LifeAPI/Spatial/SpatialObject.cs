using System;

namespace LifeAPI.Spatial {

    public class SpatialObject {
        public readonly Enum CollisionType;
        public IShapeOld Shape;

        public SpatialObject(IShapeOld shape, Enum collisionType) {
            Shape = shape;
            CollisionType = collisionType;
        }
    }

}