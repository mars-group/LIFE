using LIFE.Components.ESC.SpatialAPI.Common;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;

namespace LIFE.Components.ESC.SpatialAPI.Shape {
  
  /// <summary>
  ///   Two vectors, defining marking the vertices of an (axis aligned) bounding box.
  /// </summary>
  /// <remarks>
  ///   Notice, that the LeftBottomFront boundary values are inclusive, while the RightTopRear are exclusive.
  ///   This is to avoid ambiguity in Oct responsibility.
  /// </remarks>
  public class BoundingBox : IShape {

    private AABB _aabb;
    private Vector3 _leftBottomFront;
    private Vector3 _rightTopRear;

    /// <summary>
    ///   Creates a new bounding box by using it's edges.
    /// </summary>
    /// <param name="leftBottomFront">The edge at left, bottom and front.</param>
    /// <param name="rightTopRear">The edge at right, top and rear.</param>
    /// <returns>The created bounding box.</returns>
    public BoundingBox(Vector3 leftBottomFront, Vector3 rightTopRear) {
      _leftBottomFront = leftBottomFront;
      _rightTopRear = rightTopRear;

      var dx = RightTopRear.X - LeftBottomFront.X;
      var dy = RightTopRear.Y - LeftBottomFront.Y;
      var dz = RightTopRear.Z - LeftBottomFront.Z;

      Dimension = new Vector3(dx, dy, dz);
      Position = LeftBottomFront + new Vector3(dx/2, dy/2, dz/2);
    }

    private AABB AABB {
      get {
        if (_aabb.XIntv == null) _aabb = AABB.Generate(Position, Rotation.GetDirectionalVector(), Dimension);
        return _aabb;
      }
    }

    public Vector3 Max {
      get { return _rightTopRear; }
    }

    public Vector3 Min {
      get { return _leftBottomFront; }
    }

    public Vector3 LeftBottomFront {
      get { return _leftBottomFront; }
    }

    public Vector3 RightTopRear {
      get { return _rightTopRear; }
    }

    public Vector3 Dimension { get; }

    public double Width {
      get { return Dimension.X; }
    }

    public double Height {
      get { return Dimension.Y; }
    }

    public double Length {
      get { return Dimension.Z; }
    }

    /// <summary>
    ///   Creates a new bounding box by using it's edges.
    /// </summary>
    /// <param name="leftBottomFront">The edge at left, bottom and front.</param>
    /// <param name="rightTopRear">The edge at right, top and rear.</param>
    /// <returns>The created bounding box.</returns>
    public static BoundingBox GenerateByCorners(Vector3 leftBottomFront, Vector3 rightTopRear) {
      return new BoundingBox(leftBottomFront, rightTopRear);
    }

    /// <summary>
    ///   Creates a new bounding box by defining position and dimension.
    /// </summary>
    /// <param name="position">Defines the ceneter point of the bounding box.</param>
    /// <param name="dimension">Defines the dimensions of the bounding box.</param>
    /// <returns>The created bounding box.</returns>
    public static BoundingBox GenerateByDimension(Vector3 position, Vector3 dimension) {
      return new BoundingBox(position - dimension/2, position + dimension/2);
    }

    /// <summary>
    ///   Indicates intersection between this and the other bounding box.
    /// </summary>
    /// <param name="other">The bounding box that is checked for intersection.</param>
    /// <returns>True, if the other bounding box intersects with this bounding box. False otherwise.</returns>
    public bool IntersectsWith(BoundingBox other) {
      return AABB.Intersects(AABB, other.AABB);
    }

    /// <summary>
    ///   Returns true, if the line between the given vectors crosses through this box.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public bool Crosses(Vector3 from, Vector3 to) {
      /* Basically, for each axis a:
       *  if the start and end point's value are within our own boundaries
       *  and thesame is true for any of the other axes, the line crosses us.
       * Works only, because bounding boxes are axis aligned, i.e. no diagonal lines.
       */
      return
        ((from.X >= LeftBottomFront.X) && (to.X < RightTopRear.X) && (from.Y >= LeftBottomFront.Y) &&
         (to.Y < RightTopRear.Y))
        ||
        ((from.Z >= LeftBottomFront.Z) && (to.Z < RightTopRear.Z)) ||
        ((from.Y >= LeftBottomFront.Y) && (to.Y < RightTopRear.Y) && (from.X >= LeftBottomFront.X) &&
         (to.X < RightTopRear.X)) || ((from.Z >= LeftBottomFront.Z) && (to.Z < RightTopRear.Z)) ||
        ((from.Z >= LeftBottomFront.Z) && (to.Z < RightTopRear.Z) && (from.X >= LeftBottomFront.X) &&
         (to.X < RightTopRear.X)) || ((from.Y >= LeftBottomFront.Y) && (to.Y < RightTopRear.Y));
    }

    public bool Surrounds(Vector3 position) {
      var result = (position.X >= LeftBottomFront.X) && (position.X < RightTopRear.X) &&
                   (position.Y >= LeftBottomFront.Y) && (position.Y < RightTopRear.Y) &&
                   (position.Z >= LeftBottomFront.Z) && (position.Z < RightTopRear.Z);
      return result;
    }

    public BoundingBox Copy() {
      return GenerateByDimension(Position, Dimension);
    }

    public bool Contains(BoundingBox other) {
      return Contains(this, other);
    }

    public static bool Contains(BoundingBox box1, BoundingBox box2) {
      return (box1.LeftBottomFront <= box2.LeftBottomFront) && (box2.RightTopRear <= box1.RightTopRear);
    }

    public override string ToString() {
      return string.Format("{0}({1}->{2})", GetType().Name, Min, Max);
    }

    public static BoundingBox operator *(BoundingBox left, double right) {
      return GenerateByDimension(left.Position, left.Dimension*right);
    }

    public static BoundingBox operator /(BoundingBox left, double right) {
      return GenerateByDimension(left.Position, left.Dimension/right);
    }

    public static BoundingBox operator +(BoundingBox left, Vector3 right) {
      return GenerateByDimension(left.Position, left.Dimension + right);
    }

    public static BoundingBox operator -(BoundingBox left, Vector3 right) {
      return GenerateByDimension(left.Position, left.Dimension - right);
    }

    /// <summary>
    ///   Creates a <code>BoundingBox</code> that contains this box and the other one.
    /// </summary>
    /// <param name="other">that is used to expand this box</param>
    /// <returns>A <code>BoundingBox</code> that contains both boxes</returns>
    public BoundingBox ExpandedBy(BoundingBox other) {
      var minX = other.Min.X < Min.X ? Min.X : other.Min.X;
      var minY = other.Min.Y < Min.Y ? Min.Y : other.Min.Y;
      var minZ = other.Min.Z < Min.Z ? Min.Z : other.Min.Z;

      var maxX = other.Max.X < Max.X ? Max.X : other.Max.X;
      var maxY = other.Max.Y < Max.Y ? Max.Y : other.Max.Y;
      var maxZ = other.Max.Z < Max.Z ? Max.Z : other.Max.Z;

      return GenerateByCorners(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
    }

    /// <summary>
    ///   Creates a <code>BoundingBox</code> that is expanded by given addend in Min and Max direction.
    /// </summary>
    /// <param name="addend">that is used to expand this box</param>
    /// <returns>An expanded <code>BoundingBox</code></returns>
    public BoundingBox ExpandedBy(double addend) {
      var minX = Min.X - addend;
      var minY = Min.Y - addend;
      var minZ = Min.Z - addend;

      var maxX = Max.X + addend;
      var maxY = Max.Y + addend;
      var maxZ = Max.Z + addend;

      return GenerateByCorners(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
    }

    /// <summary>
    ///   Creates a <code>BoundingBox</code> that is expanded by given addend in Min and Max direction.
    /// </summary>
    /// <param name="position">that is used to expand this box</param>
    /// <param name="radius">that is used to expand this box</param>
    /// <returns>An expanded <code>BoundingBox</code></returns>
    public BoundingBox ExpandedBy(Vector3 position, double radius) {
      var minX = position.X - radius < Min.X ? position.X - radius : Min.X;
      var minY = position.Y - radius < Min.Y ? position.Y - radius : Min.Y;
      var minZ = position.Z - radius < Min.Z ? position.Z - radius : Min.Z;

      var maxX = position.X + radius > Max.X ? position.X + radius : Max.X;
      var maxY = position.Y + radius > Max.Y ? position.Y + radius : Max.Y;
      var maxZ = position.Z + radius > Max.Z ? position.Z + radius : Max.Z;

      return GenerateByCorners(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
    }

    protected bool Equals(BoundingBox other) {
      return RightTopRear.Equals(other.RightTopRear) && LeftBottomFront.Equals(other.LeftBottomFront);
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != GetType()) return false;
      return Equals((BoundingBox) obj);
    }

    public override int GetHashCode() {
      unchecked {
        return (_rightTopRear.GetHashCode()*397) ^ _leftBottomFront.GetHashCode();
      }
    }

    #region IShape Members

    public Vector3 Position { get; }

    public Direction Rotation {
      get { return new Direction(); }
    }

    public BoundingBox Bounds {
      get { return this; }
    }

    public bool IntersectsWith(IShape shape) {
      return IntersectsWith(shape.Bounds);
    }

    public IShape Transform(Vector3 movement, Direction rotation) {
      return new Cuboid(Dimension, Position, Rotation).Transform(movement, rotation).Bounds;
    }

    #endregion
  }
}