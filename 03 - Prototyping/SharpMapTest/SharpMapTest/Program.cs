using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
            var myMap = new Map {
                Size = rasterLayer.Size,
                MinimumZoom = 5,
                BackColor = Color.White,
            };


            VectorLayer myLayer = new VectorLayer("My layer") {
                DataSource = new SharpMap.Data.Providers.ShapeFile(@".\GISData\knp_rivers_cullum.shp"),
            };

            myMap.Layers.Add(rasterLayer);
            var featureSet = new FeatureDataSet();

            rasterLayer.ExecuteIntersectionQuery(new NetTopologySuite.Geometries.Point(rasterLayer.Size.Height / 2, rasterLayer.Size.Width /2), featureSet);

            myMap.ZoomToExtents();
            Image imgMap = myMap.GetMap();
            imgMap.Save(@".\mymap.png", ImageFormat.Png);
            Process.Start(@"E:\SoftwareProjekte\LIFE\03 - Prototyping\SharpMapTest\SharpMapTest\bin\Debug\mymap.png");
        }
    }
}
