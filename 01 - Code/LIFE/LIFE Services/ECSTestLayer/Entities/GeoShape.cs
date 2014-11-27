using GeoAPI.Geometries;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;

namespace ESCTestLayer.Entities.Spatial {
  
  public class GeoShape : IShape {
    public IGeometry Geometry { get; set; }
    
    public TVector GetPosition() {
      var x = Geometry.Centroid.Coordinate.X;
      var y = Geometry.Centroid.Coordinate.Y;
      var z = Geometry.Centroid.Coordinate.Z;
      return new TVector(x, y, z);
    }
  }
}
