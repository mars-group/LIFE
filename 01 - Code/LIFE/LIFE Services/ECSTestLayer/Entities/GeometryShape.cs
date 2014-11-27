using GeoAPI.Geometries;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;

namespace ESC.Entities {
  
  public class GeometryShape : IShape {
    public IGeometry Geometry { get; set; }
    
    public TVector GetPosition() {
      var x = Geometry.Centroid.Coordinate.X;
      var y = Geometry.Centroid.Coordinate.Y;
      var z = Geometry.Centroid.Coordinate.Z;
      return new TVector(x, y, z);
    }
  }
}
