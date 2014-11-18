using System;
using System.Diagnostics;
using System.Drawing;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using SharpMap;
using SharpMap.Data;
using SharpMap.Layers;

namespace SharpMapTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var rasterLayer = new GdalRasterLayer("elevationLayer", @"E:\SoftwareProjekte\KNPSimulation\GISData\" + @"knp_srtm90m.asc");
            
            var myMap = new Map {
                Size = rasterLayer.Size,
                MinimumZoom = 1,
                BackColor = Color.Black,
            };
            
            myMap.Layers.Add(rasterLayer);
            myMap.ZoomToExtents();
             
            var projectedCoordinate = myMap.ImageToWorld(new PointF(2000, 5000));
            var polygon =
                new Polygon(
                    new LinearRing(new Coordinate[]
                        {
                         myMap.ImageToWorld(new PointF(2000, 5000)), 
                         myMap.ImageToWorld(new PointF(4000, 5000)),
                         myMap.ImageToWorld(new PointF(2000, 7500)),
                         myMap.ImageToWorld(new PointF(4000, 7500)),
                         myMap.ImageToWorld(new PointF(2000, 5000))
                        }
                    )
                );

            
            var featureSet = new FeatureDataSet();

            var sw = Stopwatch.StartNew();
            sw.Start();
            rasterLayer.ExecuteIntersectionQuery(new NetTopologySuite.Geometries.Point(31, -23), featureSet);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            var featureSet2 = new FeatureDataSet();
            sw = Stopwatch.StartNew();
            sw.Start();

            rasterLayer.ExecuteIntersectionQuery(polygon, featureSet2);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            //myMap.ZoomToExtents();

            /**Image imgMap = myMap.GetMap();
            imgMap.Save(@".\mymap.png", ImageFormat.Png);
            Process.Start(@"E:\SoftwareProjekte\LIFE\03 - Prototyping\SharpMapTest\SharpMapTest\bin\Debug\mymap.png");
             */
            Console.ReadLine();
        }
    }
}
