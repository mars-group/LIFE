using GenericAgentArchitectureCommon.Interfaces;
using GeoAPI.Geometries;

namespace GenericAgentArchitectureCommon.Datatypes {
  
  /// <summary>
  /// This halo serves as a perception object in a spatial environment.
  /// </summary>
  public class SpatialHalo : ISpecificator {
    
    public readonly IGeometry Geometry;   // Perception object.
    private readonly int _informationType; // The information type to query.

    /// <summary>
    ///   Create a new spatial halo.
    /// </summary>
    /// <param name="geometry">Perception object.</param>
    /// <param name="informationType">The information type to query.</param>
    public SpatialHalo(IGeometry geometry, int informationType) {
      Geometry = geometry;
      _informationType = informationType;
    }


    /// <summary>
    ///   Return the information type specified by this object.
    /// </summary>
    /// <returns>Information type (as enum value).</returns>
    public int GetInformationType() {
      return _informationType;
    }
  }
}
