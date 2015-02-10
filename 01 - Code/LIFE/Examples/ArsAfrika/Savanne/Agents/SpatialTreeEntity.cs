using System;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Movement;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Shape;

namespace Savanne.Agents {
    internal class SpatialTreeEntity : ISpatialEntity {
        public SpatialTreeEntity(double x, double y,
            CollisionType collisionType = SpatialAPI.Entities.Movement.CollisionType.MassiveAgent) {
            CollisionType = collisionType;
            Shape = BoundingBox.GenerateByDimension(Vector3.Zero, new Vector3(x, y));
        }

        #region ISpatialEntity Members

        public IShape Shape { get; set; }
        public Enum CollisionType { get; private set; }
        public Guid AgentGuid { get; private set; }

        #endregion
    }
}