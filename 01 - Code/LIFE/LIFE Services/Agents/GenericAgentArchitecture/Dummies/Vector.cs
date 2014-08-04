using System;

namespace GenericAgentArchitecture.Dummies {

  /// <summary>
  /// This is a simple structure to represent a coordinate or vector in a 3D space.
  /// </summary>
  internal class Vector {

    public int X;   // Planar value 1, x-axis (left/right) 
    public int Y;   // Planar value 2, y-axis (forward/backward)
    public int Z;   // Height value (up/down)
    public bool Is3D { get; private set; }  // Is this a 3D vector?


    /// <summary>
    ///   Create a new 2D vector.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    public Vector(int x, int y) {
      X = x;
      Y = y;
      Z = 0;
      Is3D = false;
    }
    
    
    /// <summary>
    ///   Create a new 3D vector.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="z">Z coordinate.</param>
    public Vector (int x, int y, int z) {
      X = x;
      Y = y;
      Z = z;
      Is3D = true;
    }


    /// <summary>
    ///   Return the cartesian distance of two vector points. 
    /// </summary>
    /// <param name="otherVector">The point to measure the distance to.</param>
    /// <returns>The distance value.</returns>
    public double GetDistance(Vector otherVector) {
      return Math.Sqrt((X - otherVector.X)*(X - otherVector.X) +
                       (Y - otherVector.Y)*(Y - otherVector.Y) +
                       (Z - otherVector.Z)*(Z - otherVector.Z));
    }


    /// <summary>
    ///   Output the vector.
    /// </summary>
    /// <returns>String with component-based notation.</returns>
    public override string ToString() {
      return !Is3D ? String.Format("({0,2}|{1,2})", X,Y)
                   : String.Format("({0,2}|{1,2}|{2,2})", X,Y,Z);
    }
  }
}