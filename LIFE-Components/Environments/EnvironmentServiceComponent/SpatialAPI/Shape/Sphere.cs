using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;

namespace LIFE.Components.ESC.SpatialAPI.Shape {

  public class Sphere : IShape {

    private Vector3 _position;

    public Sphere(Vector3 position, double radius) {
      _position = position;
      Radius = radius;

      var aaa = new Vector3(radius, radius, radius);
      var leftBottomFront = position - aaa;
      var rightTopRear = position + aaa;

      Bounds = BoundingBox.GenerateByCorners(leftBottomFront, rightTopRear);
    }

    public double Radius { get; }

    protected bool Equals(Sphere other) {
      return Position.Equals(other.Position) && Radius.Equals(other.Radius);
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != GetType()) return false;
      return Equals((Sphere) obj);
    }

    public override int GetHashCode() {
      unchecked {
        return (_position.GetHashCode()*397) ^ Radius.GetHashCode();
      }
    }

    #region IShape Members

    public Direction Rotation {
      get { return new Direction(); }
    }

    public Vector3 Position {
      get { return _position; }
    }

    public BoundingBox Bounds { get; }

    public bool IntersectsWith(IShape shape) {
      var other = shape as Sphere;
      if (other != null) return Position.GetDistance(other.Position) < Radius + other.Radius;
      return shape.Bounds.IntersectsWith(Bounds);
    }

    public IShape Transform(Vector3 movement, Direction rotation) {
      return new Sphere(Position + movement, Radius);
    }

    public override string ToString() {
      return string.Format("Sphere({0}+-{1})", Position, Radius);
    }

    #endregion
  }
}