using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
            var rasterLayer = new GdalRasterLayer("elevationLayer", @"..\..\..\GISData\knp_srtm90m.asc");

            var myMap = new Map {
                Size = rasterLayer.Size,
                MinimumZoom = 5,
                BackColor = Color.White,
            };

            myMap.Layers.Add(rasterLayer);
            myMap.ZoomToExtents();
            var projectedCoordinate = myMap.ImageToWorld(new PointF(2000, 5000));


            var featureSet = new FeatureDataSet();
            
            rasterLayer.ExecuteIntersectionQuery(new NetTopologySuite.Geometries.Point(31, -23), featureSet);

            var featureSet2 = new FeatureDataSet();

            rasterLayer.ExecuteIntersectionQuery(new NetTopologySuite.Geometries.Point(projectedCoordinate.X, projectedCoordinate.Y) , featureSet2);

            myMap.ZoomToExtents();
            Image imgMap = myMap.GetMap();
            imgMap.Save(@".\mymap.png", ImageFormat.Png);
            Process.Start(@"E:\SoftwareProjekte\LIFE\03 - Prototyping\SharpMapTest\SharpMapTest\bin\Debug\mymap.png");
        }
    }
}
