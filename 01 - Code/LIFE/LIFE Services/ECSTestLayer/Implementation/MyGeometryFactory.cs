namespace ESCTestLayer.Implementation {
    using GeoAPI.Geometries;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.Utilities;

    public static class MyGeometryFactory {

        /// <summary>
        ///     Creates a rectangle with given dimension at given point.
        /// </summary>
        /// <param name="width">Horizontal dimension.</param>
        /// <param name="height">Vertical dimension.</param>
        /// <param name="centroid">Defines the center of this rectangle.</param>
        /// <returns>A rectangle that is an IGeometry.</returns>
        public static IGeometry Rectangle(double width, double height, Coordinate centroid) {
            var factory = new GeometricShapeFactory(new GeometryFactory(new PrecisionModel(100000d)));
            factory.Centre = centroid;
            factory.Width = width;
            factory.Height = height;
            return factory.CreateRectangle().Envelope;
        }

        /// <summary>
        ///     Creates a rectangle with given dimension at origin.
        /// </summary>
        /// <param name="width">Horizontal dimension.</param>
        /// <param name="height">Vertical dimension.</param>
        /// <returns>A rectangle that is an IGeometry.</returns>
        public static IGeometry Rectangle(double width, double height) {
            return Rectangle(width, height, new Coordinate(0, 0));
        }
    }
}