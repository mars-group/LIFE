using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using LIFE.API.GeoCommon;
using LIFE.Components.GeoPotentialFieldLayer;
using NUnit.Framework;

namespace GeoPotentialFieldLayerTests
{
    [TestFixture]
    public class GeoPotentialFieldLayerTest
    {
        private const string FileAllPositive = "./input/allPositiveGps.txt";
        private const string FileMixed = "./input/mixedGps.txt";
        private const string FileAllNegative = "./input/allNegativeGps.txt";
        private const string WpCurrent = "./input/wp_current.txt";
        private const string Elephants = "./input/elephant1989_constant_population.csv";

        private class ConcreteGeoPotentialFieldLayer : GeoPotentialFieldLayer
        {
        }


        public GeoPotentialFieldLayer GetLayerByFile(string path)
        {
            var layer = new ConcreteGeoPotentialFieldLayer();
            layer.LoadField(path);
            return layer;
        }

        [Test]
        public void ShouldGetWaterPointForEveryLeadingElephant()
        {
            var layer = GetLayerByFile(WpCurrent);

            var elephantCoords = new List<GeoCoordinate>();
            using (var reader = File.OpenText(Elephants))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    if (values[0] == "lat")
                    {
                    }
                    else
                    {
                        if (bool.Parse(values[4]))
                            elephantCoords.Add(new GeoCoordinate(double.Parse(values[0], CultureInfo.InvariantCulture),
                                double.Parse(values[1], CultureInfo.InvariantCulture)));
                    }
                }
            }

            Parallel.ForEach(elephantCoords,
                coord =>
                {
                    /*var cell =*/
                    layer.ExploreClosestWithEndlessSight(coord.Latitude, coord.Longitude);
                    //Console.WriteLine($"Found cell {cell}");
                });
        }


        [Test]
        public void ShouldReturnFieldPosition()
        {
            var layer = GetLayerByFile(FileAllPositive);
            var position = layer.GetFieldPositionByCoordinate(3.5, 1.5);
            Assert.AreEqual(1, position);
        }

        [Test]
        public void ShouldReturnGpsCell()
        {
            var layer = GetLayerByFile(FileAllPositive);
            var gpsForCell = layer.GetGpsForCenterOfCell(0);
            Assert.AreEqual(new GeoCoordinate(3.7625, 0.525), gpsForCell);
        }

        [Test]
        public void ShouldReturnGpsCellInMixedSignGpsArea()
        {
            var layer = GetLayerByFile(FileMixed);
            var gpsForCell = layer.GetGpsForCenterOfCell(0);
            Assert.AreEqual(new GeoCoordinate(1.5, -1.5), gpsForCell);
        }

        [Test]
        public void ShouldReturnGpsCellInNegativeGpsArea()
        {
            var layer = GetLayerByFile(FileAllNegative);
            var gpsForCell = layer.GetGpsForCenterOfCell(0);
            Assert.AreEqual(new GeoCoordinate(-1.5, -4.5), gpsForCell);
        }
    }
}