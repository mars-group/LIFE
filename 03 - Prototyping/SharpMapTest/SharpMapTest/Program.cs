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
            var rasterLayer = new GdalRasterLayer("elevationLayer", @".\GISData\knp_srtm90m.asc");
            new VectorLayer("MyLayer");
            var myMap = new Map {
                Size = rasterLayer.Size,
                MinimumZoom = 5,
                BackColor = Color.White,
            };

            myMap.Layers.Add(rasterLayer);
            
            var featureSet = new FeatureDataSet();
            
            rasterLayer.ExecuteIntersectionQuery(new NetTopologySuite.Geometries.Point(31, -23), featureSet);
            
            myMap.ZoomToExtents();
            Image imgMap = myMap.GetMap();
            imgMap.Save(@".\mymap.png", ImageFormat.Png);
            Process.Start(@"E:\SoftwareProjekte\LIFE\03 - Prototyping\SharpMapTest\SharpMapTest\bin\Debug\mymap.png");
        }
    }
}
