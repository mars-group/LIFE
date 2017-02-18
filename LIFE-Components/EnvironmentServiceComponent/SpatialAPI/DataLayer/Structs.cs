using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;

namespace LIFE.Components.ESC.SpatialAPI.DataLayer {


  /// <summary>
  ///   Enum for coordinates axes.
  /// </summary>
  public enum Axis {
    
    /// <summary>
    ///   X axis [Plane: left / right].
    /// </summary>
    X, 

    /// <summary>
    ///   Y axis. [Plane: in / out]
    /// </summary>
    Y, 
    
    /// <summary>
    ///   Z axis. [Height: up / down]
    /// </summary>
    Z
  }


  /// <summary>
  ///   Defines a range from (and including) start to (and including) end.
  /// </summary>
  public struct Range {

    /// <summary>
    ///   The axis this range references to.
    /// </summary>
    public Axis Axis;

    /// <summary>
    ///   Beginning of the range (first index).
    /// </summary>
    public int Start;
    
    /// <summary>
    ///   End of the range (last index).
    /// </summary>
    public int End;
  }


  /// <summary>
  ///   Defines a query shape (specified by a position and an aligned geometry object).
  /// </summary>
  public struct ShapeQuery {
    
    /// <summary>
    ///   Center point of this shape.
    /// </summary>
    public Vector3 Position;
    
    /// <summary>
    ///   Alignment of shape.
    /// </summary>
    public Vector3 Orientation;
    
    /// <summary>
    ///   This shape's geometric form.
    /// </summary>
    public ISpatialEntity QueryMask;
  }
}
