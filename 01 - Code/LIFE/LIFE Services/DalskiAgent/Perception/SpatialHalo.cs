using System;
using GeoAPI.Geometries;
using LayerAPI.Perception;

namespace DalskiAgent.Perception {
  
  /// <summary>
  /// This halo serves as a perception object in a spatial environment.
  /// </summary>
  public class SpatialHalo : ISpecification {
    
    public readonly IGeometry Geometry;     // Perception object.
    private readonly Enum _informationType; // The information type to perceive.

    /// <summary>
    ///   Create a new spatial halo.
    /// </summary>
    /// <param name="geometry">Perception object.</param>
    /// <param name="informationType">The information type to perceive.</param>
    public SpatialHalo(IGeometry geometry, Enum informationType) {
      Geometry = geometry;
      _informationType = informationType;
    }


    /// <summary>
    ///   Return the information type specified by this object.
    /// </summary>
    /// <returns>Information type (as enum value).</returns>
    public Enum GetInformationType() {
      return _informationType;
    }
  }
}
