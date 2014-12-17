using System;
using EnvironmentServiceComponent.Entities.Shape;
using EnvironmentServiceComponent.Implementation;
using LifeAPI.Spatial;

namespace ESCTest.Entities {

    internal class GeometryAgent : ISpatialEntity {
        private readonly Enum _collisionType;

        public GeometryAgent(double x, double y) :
            this(x, y, CollisionType.MassiveAgent) {}


        public GeometryAgent(double x, double y, Enum collisionType) {
            _collisionType = collisionType;
            Shape = new ExploreShape(MyGeometryFactory.Rectangle(x, y));
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

        #region ISpatialEntity Members

        public Enum GetCollisionType() {
            return _collisionType;
        }


        public Enum GetInformationType() {
            throw new NotImplementedException();
        }

        public IShape Shape { get; set; }

        #endregion
    }

}