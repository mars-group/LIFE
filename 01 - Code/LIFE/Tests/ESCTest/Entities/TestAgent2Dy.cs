namespace ESCTest.Entities {
    using ESCTestLayer.Implementation;
    using GenericAgentArchitectureCommon.Interfaces;
    using GeoAPI.Geometries;
    using NetTopologySuite.Geometries;

    internal class TestAgent2D : ISpatialEntity {

        public TestAgent2D(int dx, int dy) {
            Geometry = MyGeometryFactory.Rectangle(dx, dy);
//            Point bottomLeft = new Point(0, 0, 0);
//            Point topLeft = new Point(0, dy, 0);
//            Point topRight = new Point(dx, dy, 0);
//            Point bottomRight = new Point(dx, 0, 0);
//
//            MultiPoint mp = new MultiPoint(new IPoint[] { bottomLeft, topLeft, topRight, bottomRight });
//
//            LinearRing lr = new LinearRing(new Coordinate[]{
//                                                bottomLeft.Coordinate, 
//                                                topLeft.Coordinate,
//                                                topRight.Coordinate,
//                                                bottomRight.Coordinate, 
//                                                bottomLeft.Coordinate });
//            Geometry = new Polygon(lr);
        }


        public IGeometry Geometry { get; set; }
    }
}