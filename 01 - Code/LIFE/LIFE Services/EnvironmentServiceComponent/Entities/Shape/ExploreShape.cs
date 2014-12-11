using GeoAPI.Geometries;

namespace EnvironmentServiceComponent.Entities.Shape {

    public class ExploreShape : GeometryShape
    {
        public ExploreShape(IGeometry geometry)
        {
            Geometry = geometry;
        }
    }
}
