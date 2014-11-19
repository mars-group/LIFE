namespace ESCTestLayer.Implementation {
    using GeoAPI.Geometries;
    using NetTopologySuite.Utilities;

    public static class MyGeometryFactory {

        /// <summary>
        ///     creates a rectangle with given dimension
        /// </summary>
        /// <param name="width">horizontal dimension</param>
        /// <param name="height">vertical dimension</param>
        /// <returns>a rectangle that is an IGeometry</returns>
        public static IGeometry Rectangle(double width, double height) {
            var factory = new GeometricShapeFactory();
            factory.Centre = new Coordinate(0, 0);
            factory.Width = width;
            factory.Height = height;
            return factory.CreateRectangle().Envelope;
        }
    }
}