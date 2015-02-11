using System;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Shape;

namespace EnvironmentServiceComponent.Entities {

    public class ExploreEntity : ISpatialEntity {
        private readonly Enum _collisionType;
        private readonly IShape _shape;

        public ExploreEntity(IShape shape, Enum collisionType = null) {
            _shape = shape;
            _collisionType = collisionType ?? SpatialAPI.Entities.Movement.CollisionType.MassiveAgent;
        }

        public ExploreEntity(ISpatialObject spatialObject, Enum collisionType = null) {
            _shape = spatialObject.Shape;
            _collisionType = collisionType ?? SpatialAPI.Entities.Movement.CollisionType.MassiveAgent;
        }

        public ExploreEntity(ISpatialEntity spatialEntity) {
            _shape = spatialEntity.Shape;
            _collisionType = spatialEntity.CollisionType;
        }

        public IShape Shape { get { return _shape; } set { throw new NotImplementedException(); } }
        public Enum CollisionType { get { return _collisionType; } }
        public Guid AgentGuid { get { throw new NotImplementedException(); } }

        public static ExploreEntity operator *(ExploreEntity left, double right) {
            return new ExploreEntity(left.Shape.Bounds*right, left.CollisionType);
        }

        public static ExploreEntity operator /(ExploreEntity left, double right) {
            return new ExploreEntity(left.Shape.Bounds/right, left.CollisionType);
        }

        public static ExploreEntity operator +(ExploreEntity left, Vector3 right) {
            return new ExploreEntity(left.Shape.Bounds + right, left.CollisionType);
        }

        public static ExploreEntity operator -(ExploreEntity left, Vector3 right) {
            return new ExploreEntity(left.Shape.Bounds - right, left.CollisionType);
        }
    }

}