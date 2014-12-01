using GeoAPI.Geometries;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;

namespace EnvironmentServiceComponent.Entities.Shape {

    public class GeometryShape : IShape {
        public IGeometry Geometry { get; set; }

        #region IShape Members

        public TVector GetPosition() {
            double x = Geometry.Centroid.Coordinate.X;
            double y = Geometry.Centroid.Coordinate.Y;
            double z = Geometry.Centroid.Coordinate.Z;
            if (double.IsNaN(z)) {
                z = 0.0f;
            }
            return new TVector(x, y, z);
        }

        #endregion
    }

}