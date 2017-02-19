using System;
using System.Collections.Generic;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.ESC.SpatialAPI.Common {
  
  /// <summary>
  ///   The <code>EnvBoundaryType</code> defines restrictions/behaviour of moving Entities pass a boundary.
  /// </summary>
  public enum EnvBoundaryType {
    NoRestriction,
    BlockOnExit
    //JumpToOtherSide,
  }


  /// <summary>
  ///   The <code>BoundarySpecification</code> defines a boundary border and a type of behaviour for what should happen if
  ///   an entity crosses the border.
  /// </summary>
  public class BoundarySpecification {

    public BoundarySpecification() : this(EnvBoundaryType.NoRestriction) {}

    public BoundarySpecification(EnvBoundaryType envBoundaryType, Vector3 boundary = default(Vector3)) {
      EnvBoundaryType = envBoundaryType;
      Boundary = boundary;
      BoundaryEntityInList = new List<MovementBlockingBoundaryEntity>();
      BoundaryEntityInList.Add(new MovementBlockingBoundaryEntity(Boundary));
    }

    public Vector3 Boundary { get; }
    public EnvBoundaryType EnvBoundaryType { get; private set; }
    public List<MovementBlockingBoundaryEntity> BoundaryEntityInList { get; }
  }


  /// <summary>
  ///   The <code>MovementBlockingBoundaryEntity</code> is an <code>ISpatialEntity</code> that defines a collision entity
  ///   that occurs if the boundary is crossed but should not.
  /// </summary>
  public class MovementBlockingBoundaryEntity : ISpatialEntity {
    private readonly BoundingBox _shape;

    public MovementBlockingBoundaryEntity(Vector3 boundary) {
      _shape = BoundingBox.GenerateByCorners(Vector3.Zero, boundary);
      AgentGuid = Guid.NewGuid();
    }

    public IShape Shape {
      get { return _shape; }
      set { throw new NotImplementedException(); }
    }

    public Enum CollisionType {
      get { return Entities.Movement.CollisionType.MassiveAgent; }
    }

    public Type AgentType {
      get { return GetType(); }
    }

    public Guid AgentGuid { get; }

    public override string ToString() {
      return string.Format("{0}{1}", GetType(), _shape.RightTopRear);
    }
  }
}