using System;

namespace LayerAPI.Spatial {

  /// <summary>
  ///   This class serves as a representation of a vector or vertex.
  /// </summary>
  public class Vector : IEquatable<Vector>, IEquatable<TVector> {

    private readonly bool _is3D;  // Dimension flag: false: 2D, true: 3D.
    /// <summary>
    ///   The vector's coordinates.
    /// </summary>
    public double X, Y, Z;


    /// <summary>
    ///   Initialize a two-dimensional vector (height is set to zero). 
    /// </summary>
    public Vector (double x, double y) {
      X = x;
      Y = y;
      Z = 0.0d;
      _is3D = false;
    }


    /// <summary>
    ///   Initialize a three-dimensional vector.
    /// </summary>
    public Vector (double x, double y, double z) {
      X = x;
      Y = y;
      Z = z;
      _is3D = true;
    }


    /// <summary>
    ///   Calculate point-to-point distance.
    /// </summary>
    /// <param name="pos">The target point.</param>
    /// <returns>Euclidian distance value.</returns>
    public double GetDistance(Vector pos) {
      return Math.Sqrt((X - pos.X)*(X - pos.X) +
                               (Y - pos.Y)*(Y - pos.Y) +
                               (Z - pos.Z)*(Z - pos.Z));      
    }


    /// <summary>
    ///   Calculate the vector length.
    /// </summary>
    /// <returns>Length of this vector.</returns>
    public double GetLength() {
      return GetDistance(new Vector(0.0d, 0.0d, 0.0d));
    }


    /// <summary>
    ///   Calculate the normalized vector.
    /// </summary>
    /// <returns>The normalized vector.</returns>
    public Vector GetNormalVector() {
      var length = GetLength();
      return new Vector(X/length, Y/length, Z/length);
    }


    /// <summary>
    ///   Create normalized vectors orthogonal to this one.
    /// </summary>
    /// <param name="nY">Pointer for new y-axis normal vector.</param>
    /// <param name="nZ">Same for z-axis (height) vector.</param>
    public void GetPlanarOrthogonalVectors(out Vector nY, out Vector nZ) {
      
      // [Y-Axis]: Create orthogonal vector to new x-axis laying in plane (x, y): => Scalar product = 0.
      nY = new Vector(-Y/X, 1.0d, 0.0d).GetNormalVector();

      // [Z-Axis / Height]: Build orthogonal vector with cross-product.
      var x3 = (Y * nY.Z  -  Z * nY.Y);  // x: a2b3 - a3b2
      var y3 = (Z * nY.X  -  X * nY.Z);  // y: a3b1 - a1b3
      var z3 = (X * nY.Y  -  Y * nY.X);  // z: a1b2 - a2b1
      nZ = new Vector(x3, y3, z3).GetNormalVector();
    }


    /// <summary>
    ///   Output the position.
    /// </summary>
    /// <returns>String with component-based notation.</returns>
    public override string ToString() {
      return !_is3D ? String.Format("({0,5:0.00}|{1,5:0.00})",            X,Y)
                    : String.Format("({0,5:0.00}|{1,5:0.00}|{2,5:0.00})", X,Y,Z);
    }


    /// <summary>
    ///   Compare this with another Vector object for equality.
    /// </summary>
    /// <param name="vector">Comparison vector.</param>
    /// <returns>Equals boolean.</returns>
    public bool Equals(Vector vector) {
      return (Math.Abs(X - vector.X) <= double.Epsilon &&
              Math.Abs(Y - vector.Y) <= double.Epsilon &&
              Math.Abs(Z - vector.Z) <= double.Epsilon);
    }


    /// <summary>
    ///   Compare this with another TVector object for equality.
    /// </summary>
    /// <param name="tvector">Comparison vector.</param>
    /// <returns>Equals boolean.</returns>
    public bool Equals(TVector tvector) {
      return (Math.Abs(X - tvector.X) <= double.Epsilon &&
              Math.Abs(Y - tvector.Y) <= double.Epsilon &&
              Math.Abs(Z - tvector.Z) <= double.Epsilon);
    }

    /// <summary>
    ///   Casts this Vector to a TVector object.
    /// </summary>
    /// <returns>TVector equivalent.</returns>
    public TVector GetTVector() {
      return new TVector(X,Y,Z);
    }


    /// <summary>
    ///   Calculate the dot (scalar) product of two vectors.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>The scalar [inner] product.</returns>
    public static double DotProduct(Vector a, Vector b) {
      return a.X*b.X + a.Y*b.Y + a.Z*b.Z;
    }


    /// <summary>
    ///   Calcute the cross product of two vectors.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>The cross [outer] product.</returns>
    public static Vector CrossProduct(Vector a, Vector b) {
      return new Vector(a.Y*b.Z - a.Z*b.Y, a.Z*b.X - a.X*b.Z, a.X*b.Y - a.Y*b.X);
    }


    /// <summary>
    ///   Addition of two vectors.
    /// </summary>
    /// <param name="left">First (left) vector.</param>
    /// <param name="right">Second (right) vector.</param>
    /// <returns>Sum of both vectors.</returns>
    public static Vector operator +(Vector left, Vector right) {
      return new Vector(left.X+right.X, left.Y+right.Y, left.Z+right.Z);
    }


    /// <summary>
    ///   Inverse (* -1) of a vector.
    /// </summary>
    /// <param name="right">Inversed vector.</param>
    /// <returns>Vector *(-1).</returns>
    public static Vector operator -(Vector right) {
      return new Vector(-right.X, -right.Y, -right.Z);
    }


    /// <summary>
    ///   Substraction of two vectors.
    /// </summary>
    /// <param name="left">First (left) vector.</param>
    /// <param name="right">Second (right) vector.</param>
    /// <returns>Difference of both vectors.</returns>    
    public static Vector operator -(Vector left, Vector right) {
      return new Vector(left.X-right.X, left.Y-right.Y, left.Z-right.Z);
    }


    /// <summary>
    ///   Multiply a vector with a scalar.
    /// </summary>
    /// <param name="vec">Vector to scale.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>The scaled vector.</returns> 
    public static Vector operator *(Vector vec, double factor) {
      return new Vector(vec.X*factor, vec.Y*factor, vec.Z*factor);
    }


    /// <summary>
    ///   Multiply a vector with a scalar.
    /// </summary>
    /// <param name="factor">Scaling factor.</param>
    /// <param name="vec">Vector to scale.</param>
    /// <returns>The scaled vector.</returns> 
    public static Vector operator *(double factor, Vector vec) {
      return new Vector(vec.X*factor, vec.Y*factor, vec.Z*factor);
    }

    
    /// <summary>
    ///   Divides a vector with a scalar.
    ///   (Multiplication with inversed divisor.) 
    /// </summary>
    /// <param name="vec">Vector to scale.</param>
    /// <param name="div">Divisor scalar.</param>
    /// <returns>The scaled vector.</returns>
    public static Vector operator /(Vector vec, double div) {
      return vec*(1.0d/div);
    }
  };
}
