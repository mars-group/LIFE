using System;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   This class serves as a representation of a vector or vertex.
  /// </summary>
  public struct VectorDeprecated {

    private bool _is3D;      // Dimension flag: false: 2D, true: 3D.
    public float X, Y, Z;    // The vector's coordinates.


    /// <summary>
    ///   Initialize a two-dimensional vector (height is set to zero). 
    /// </summary>
    public VectorDeprecated (float x, float y) {
      X = x;
      Y = y;
      Z = 0;
      _is3D = false;
    }


    /// <summary>
    ///   Initialize a three-dimensional vector.
    /// </summary>
    public VectorDeprecated (float x, float y, float z) {
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
    public float GetDistance(VectorDeprecated pos) {
      return (float) Math.Sqrt((X - pos.X)*(X - pos.X) +
                               (Y - pos.Y)*(Y - pos.Y) +
                               (Z - pos.Z)*(Z - pos.Z));      
    }


    /// <summary>
    ///   Calculate the vector length.
    /// </summary>
    /// <returns>Length of this vector.</returns>
    public float GetLength() {
      return GetDistance(new VectorDeprecated(0.0f, 0.0f, 0.0f));
    }


    /// <summary>
    ///   Calculate the normalized vector.
    /// </summary>
    /// <returns>The normalized vector.</returns>
    public VectorDeprecated GetNormalVector() {
      var length = GetLength();
      return new VectorDeprecated(X/length, Y/length, Z/length);
    }


    /// <summary>
    ///   Create normalized vectors orthogonal to this one.
    /// </summary>
    /// <param name="nY">Pointer for new y-axis normal vector.</param>
    /// <param name="nZ">Same for z-axis (height) vector.</param>
    public void GetPlanarOrthogonalVectors(out VectorDeprecated nY, out VectorDeprecated nZ) {
      
      // [Y-Axis]: Create orthogonal vector to new x-axis laying in plane (x, y): => Scalar product = 0.
      nY = new VectorDeprecated(-Y/X, 1.0f, 0.0f).GetNormalVector();

      // [Z-Axis / Height]: Build orthogonal vector with cross-product.
      var x3 = (Y * nY.Z  -  Z * nY.Y);  // x: a2b3 - a3b2
      var y3 = (Z * nY.X  -  X * nY.Z);  // y: a3b1 - a1b3
      var z3 = (X * nY.Y  -  Y * nY.X);  // z: a1b2 - a2b1
      nZ = new VectorDeprecated(x3, y3, z3).GetNormalVector();
    }


    /// <summary>
    ///   Output the position.
    /// </summary>
    /// <returns>String with component-based notation.</returns>
    public override string ToString() {
      return !_is3D ? String.Format("({0,5:0.00}|{1,5:0.00})",            X,Y)
                    : String.Format("({0,5:0.00}|{1,5:0.00}|{2,5:0.00})", X,Y,Z);
    }
  };
}
