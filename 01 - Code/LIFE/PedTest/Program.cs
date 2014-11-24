using System;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace PedTest
{
    class Program
    {
        static void Main() {
            Console.SetBufferSize(160, 50);
            Console.SetWindowSize(160, 50);

            // 1) FLOATING DOUBLE PRECISION
            Console.WriteLine("Test 1");
            IGeometryFactory gfactory = GeometryFactory.Default;
            WKTReader greader = new WKTReader(gfactory);
            IGeometry poly1 = greader.Read("POLYGON((9.9999996181577444 5.5, 9.9999996181577444 10, 10.049999618902803 10, 10.049999618902803 5.5, 9.9999996181577444 5.5))");
            IGeometry poly2 = greader.Read("POLYGON((0 9.9999996181577444, 0 10.049999618902803, 10 10.049999618902803, 10 9.9999996181577444, 0 9.9999996181577444))");
            
            Console.WriteLine(poly1);
            Console.WriteLine(poly2);

            if (!poly1.Intersects(poly2)) Console.WriteLine("Doesn't intersect -> No collision!");
            if (poly1.Touches(poly2)) Console.WriteLine("Touches -> No collision!");
            if (poly1.Intersects(poly2) && !poly1.Touches(poly2)) {
                Console.WriteLine("Intersects and doesn't touch -> Collision!");
                Console.WriteLine("Intersection: " + poly1.Intersection(poly2));
            }
            Console.WriteLine();

            // 2) FLOATING SINGLE PRECISION
            Console.WriteLine("Test 2");
            gfactory = GeometryFactory.FloatingSingle;
            poly1 = gfactory.CreatePolygon(poly1.Coordinates);
            poly2 = gfactory.CreatePolygon(poly2.Coordinates);

            Console.WriteLine(poly1);
            Console.WriteLine(poly2);

            if (!poly1.Intersects(poly2)) Console.WriteLine("Doesn't intersect -> No collision!");
            if (poly1.Touches(poly2)) Console.WriteLine("Touches -> No collision!");
            if (poly1.Intersects(poly2) && !poly1.Touches(poly2))
            {
                Console.WriteLine("Intersects and doesn't touch -> Collision!");
                Console.WriteLine("Intersection: " + poly1.Intersection(poly2));
            }
            Console.WriteLine();

            // 3) Warum sind die Polygone hier anders als bei 2)?
            Console.WriteLine("Test 3");
            gfactory = GeometryFactory.FloatingSingle;
            greader = new WKTReader(gfactory);
            poly1 = greader.Read("POLYGON((9.9999996181577444 5.5, 9.9999996181577444 10, 10.049999618902803 10, 10.049999618902803 5.5, 9.9999996181577444 5.5))");
            poly2 = greader.Read("POLYGON((0 9.9999996181577444, 0 10.049999618902803, 10 10.049999618902803, 10 9.9999996181577444, 0 9.9999996181577444))");

            Console.WriteLine(poly1);
            Console.WriteLine(poly2);

            if (!poly1.Intersects(poly2)) Console.WriteLine("Doesn't intersect -> No collision!");
            if (poly1.Touches(poly2)) Console.WriteLine("Touches -> No collision!");
            if (poly1.Intersects(poly2) && !poly1.Touches(poly2))
            {
                Console.WriteLine("Intersects and doesn't touch -> Collision!");
                Console.WriteLine("Intersection: " + poly1.Intersection(poly2));
            }
            Console.ReadLine();
        }

    }
}
