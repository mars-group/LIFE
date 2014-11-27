using GeoAPI.Geometries;

namespace EnvironmentServiceComponent.Entities
{
    public class ExploreShape : GeometryShape
    {
        public ExploreShape(IGeometry geometry)
        {
            Geometry = geometry;
        }
    }
}
