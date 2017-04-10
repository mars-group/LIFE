using System;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.ESC.SpatialAPI.Entities
{
    public class ExploreEntity : ISpatialEntity
    {
        public ExploreEntity(double radius)
        {
            Shape = new Sphere(Vector3.Zero, radius);
            CollisionType = Movement.CollisionType.MassiveAgent;
            AgentGuid = Guid.NewGuid();
        }

        public ExploreEntity(IShape shape, Enum collisionType = null)
        {
            Shape = shape;
            CollisionType = collisionType ?? Movement.CollisionType.MassiveAgent;
            AgentGuid = Guid.NewGuid();
        }

        public ExploreEntity(ISpatialEntity spatialEntity)
        {
            Shape = spatialEntity.Shape;
            CollisionType = spatialEntity.CollisionType;
            AgentGuid = Guid.NewGuid();
        }

        public IShape Shape { get; set; }
        public Enum CollisionType { get; }

        public Guid AgentGuid { get; }

        public Type AgentType
        {
            get { return GetType(); }
        }

        public static ExploreEntity operator *(ExploreEntity left, double right)
        {
            return new ExploreEntity(left.Shape.Bounds * right, left.CollisionType);
        }

        public static ExploreEntity operator /(ExploreEntity left, double right)
        {
            return new ExploreEntity(left.Shape.Bounds / right, left.CollisionType);
        }

        public static ExploreEntity operator +(ExploreEntity left, Vector3 right)
        {
            return new ExploreEntity(left.Shape.Bounds + right, left.CollisionType);
        }

        public static ExploreEntity operator -(ExploreEntity left, Vector3 right)
        {
            return new ExploreEntity(left.Shape.Bounds - right, left.CollisionType);
        }
    }
}