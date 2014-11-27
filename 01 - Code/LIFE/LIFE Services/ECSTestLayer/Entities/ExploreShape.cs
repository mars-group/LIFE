using GeoAPI.Geometries;

namespace ESC.Entities
{
    public class ExploreShape : GeometryShape
    {
        public ExploreShape(IGeometry geometry)
        {
            Geometry = geometry;
        }
    }
}
