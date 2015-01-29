namespace DalskiAgent.Auxiliary.Environment {
  
  /// <summary>
  ///   Two-dimensional float structure.
  /// </summary>
  public struct Float2 {
    public readonly float X, Y;
    public Float2(float x, float y) {
      X = x;
      Y = y;
    }
  };


  /// <summary>
  ///   Utility class with various collision detection functions.
  /// </summary>
  public static class CDF {
    
    /// <summary>
    ///   Function to check the intersection of two rectangles.
    /// </summary>
    /// <param name="pos1">Reference point (bottom,left) of rectangle 1.</param>
    /// <param name="span1">Dimension (extent) of the first rectangle.</param>
    /// <param name="pos2">Reference point (bottom,left) of rectangle 2.</param>
    /// <param name="span2">Dimension (extent) of the second rectangle.</param>
    /// <returns>'True', if the rectangles intersect or one contains the other.</returns>
    public static bool IntersectRects(Float2 pos1, Float2 span1, Float2 pos2, Float2 span2) {
      return IntervalsCollide(new Float2(pos1.X, pos1.X + span1.X), new Float2(pos2.X, pos2.X + span2.X)) &&
             IntervalsCollide(new Float2(pos1.Y, pos1.Y + span1.Y), new Float2(pos2.Y, pos2.Y + span2.Y));
    }


    /// <summary>
    ///   Checks, if two intervals collide.
    /// </summary>
    /// <param name="intv1">First interval.</param>
    /// <param name="intv2">Second interval.</param>
    /// <returns>'True', if the intervals overlap, touch (!) or one contains the other.</returns>
    public static bool IntervalsCollide(Float2 intv1, Float2 intv2) {
      return !(intv2.X >= intv1.Y || intv1.X >= intv2.Y);
    }
  }
}
