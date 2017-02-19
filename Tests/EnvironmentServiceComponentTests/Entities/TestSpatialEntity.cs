using System;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;
using Newtonsoft.Json;
using CT = LIFE.Components.ESC.SpatialAPI.Entities.Movement.CollisionType;

namespace EnvironmentServiceComponentTests.Entities {

  [Serializable]
  public class TestSpatialEntity : ISpatialEntity {

    private readonly CT _collisionType;
    private readonly int _id;

    public TestSpatialEntity
      (int id, Vector3 dimension, CT collisionType = CT.MassiveAgent)
      : this(BoundingBox.GenerateByCorners(Vector3.Zero, dimension), collisionType) {
      _id = id;
      AgentGuid = Guid.NewGuid();
    }

    public TestSpatialEntity
      (Vector3 dimension, CT collisionType = CT.MassiveAgent)
      : this(BoundingBox.GenerateByCorners(Vector3.Zero, dimension), collisionType) {
      AgentGuid = Guid.NewGuid();
    }

    public TestSpatialEntity
      (double x, double y, CT collisionType = CT.MassiveAgent)
      : this(new Vector3(x, y), collisionType) {
      AgentGuid = Guid.NewGuid();
    }

    [JsonConstructor]
    public TestSpatialEntity
      (BoundingBox bounds, CT collisionType = CT.MassiveAgent) {
      _collisionType = collisionType;
      Shape = bounds;
      AgentGuid = Guid.NewGuid();
    }

    public TestSpatialEntity
      (IShape bounds, CT collisionType = CT.MassiveAgent) {
      _collisionType = collisionType;
      Shape = bounds;
      AgentGuid = Guid.NewGuid();
    }

    public IShape Shape { get; set; }

    public Enum CollisionType {
      get { return _collisionType; }
    }

    public Guid AgentGuid { get; }

    public Type AgentType {
      get { return GetType(); }
    }

    public override string ToString() {
      return "$" + _id + "(" + Shape.Bounds + ")";
    }

    protected bool Equals(TestSpatialEntity other) {
      return Equals(Shape, other.Shape) && AgentGuid.Equals(other.AgentGuid) && (_collisionType == other._collisionType);
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != GetType()) return false;
      return Equals((TestSpatialEntity) obj);
    }

    public override int GetHashCode() {
      unchecked {
        var hashCode = Shape != null ? Shape.GetHashCode() : 0;
        hashCode = (hashCode*397) ^ AgentGuid.GetHashCode();
        hashCode = (hashCode*397) ^ (int) _collisionType;
        return hashCode;
      }
    }
  }
}