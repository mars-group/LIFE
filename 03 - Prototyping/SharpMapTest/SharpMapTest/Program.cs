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
            var rasterLayer = new GdalRasterLayer("elevationLayer", @"..\..\..\GISData\knp_srtm90m.asc");
            var _map = new Map {
                Size = rasterLayer.Size,
                MinimumZoom = 1
            };
            _map.Layers.Add(rasterLayer);

            var featureSet = new FeatureDataSet();

            var sw = Stopwatch.StartNew();
            sw.Start();
            var env = rasterLayer.Envelope;
            rasterLayer.ExecuteIntersectionQuery(new NetTopologySuite.Geometries.Point(31.331, -25.292), featureSet);
            sw.Stop();
            


            foreach (var table in featureSet.Tables) {
                foreach (FeatureDataRow row in table.Rows) {
                    Console.WriteLine(row.ItemArray[2]);
                }
            }

            Console.ReadLine();
        }
    }
}
