using System;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.ESC.SpatialAPI.Entities {

  /// <summary>
  ///   An object that is described by it's shape.
  /// </summary>
  public interface ISpatialObject {


    /// <summary>
    ///   Describes the spatial expansion in a defined form.
    /// </summary>
    IShape Shape { get; set; }


    /// <summary>
    ///   Return the information type specified by this object.
    /// </summary>
    Enum CollisionType { get; }
  }
}