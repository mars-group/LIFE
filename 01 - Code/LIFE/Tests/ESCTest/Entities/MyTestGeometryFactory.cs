namespace ESCTest.Entities {
    using System;
    using System.Reflection;
    using CommonTypes.TransportTypes;
    using ESC.Interface;
    using GenericAgentArchitectureCommon.Interfaces;
    using GeoAPI.Geometries;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.Geometries.Utilities;
    using NetTopologySuite.Shape;
    using NetTopologySuite.Utilities;

    internal class MyTestGeometryFactory  {

        public MyTestGeometryFactory() {
            var factory = new GeometricShapeFactory();
            factory.Centre = new Coordinate(1, 1);
            factory.Width = 4;
            factory.Height = 2;
            var rect = factory.CreateRectangle().Envelope;
            Console.WriteLine(rect);


            GeometricShapeFactory gsf2 = new GeometricShapeFactory();
            var center = rect.Centroid.Coordinate;
            AffineTransformation trans = new AffineTransformation();
            trans.SetToRotation(ConvertDegreesToRadians(90), center.X, center.Y);
            Console.WriteLine("--");
            Console.WriteLine(rect.Centroid.Coordinate);
            IGeometry result = trans.Transform(rect);
            Console.WriteLine(result);
            Console.WriteLine(result.Centroid.Coordinate);
            
            factory.Rotation = ConvertDegreesToRadians(90);
            var rect2 = factory.CreateRectangle().Envelope;
            Console.WriteLine(rect2);
        }

        public static double ConvertDegreesToRadians(double degrees)
        {
            return (Math.PI / 180) * degrees;
        }
    }
}