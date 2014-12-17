using System;

namespace LifeAPI.Spatial {

    public class SpatialEntity : SpatialObject {
        public readonly Type EntityType;
        public readonly Guid EntityGuid;

        public SpatialEntity(Type entityType, Guid entityGuid, IShapeOld shape, Enum collisionType)
            : base(shape, collisionType) {
            EntityType = entityType;
            EntityGuid = entityGuid;
        }
    }

}