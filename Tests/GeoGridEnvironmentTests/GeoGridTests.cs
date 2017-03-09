// // /*******************************************************
// //  * Copyright (C) Christian Hüning - All Rights Reserved
// //  * Unauthorized copying of this file, via any medium is strictly prohibited
// //  * Proprietary and confidential
// //  * This file is part of the MARS LIFE project, which is part of the MARS System
// //  * More information under: http://www.mars-group.org
// //  * Written by Christian Hüning <christianhuening@gmail.com>, 30.07.2016
// //  *******************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LIFE.API.Environment.GeoCommon;
using LIFE.Components.Environments.GeoGridEnvironment;
using NUnit.Framework;

// ReSharper disable AccessToModifiedClosure

namespace GeoGridEnvironmentTests
{
    [TestFixture]
    public class GeoGridTests
    {
        [SetUp]
        public void Setup()
        {
            _geoGrid = new GeoGridEnvironment<Tree>(-22.1593, -25.7295, 30.7237, 32.232578, 1000);
            //_geoGrid = new GeoGridEnvironment<Tree>(-22.3593, -25.5295, 30.9237, 32.032578, 1000);
        }

        private GeoGridEnvironment<Tree> _geoGrid;
        private readonly Random _random = new Random();

        [Test]
        public void TestExplore()
        {
            var treeCount = 1400000;
            Parallel.For(0, treeCount,
                i => _geoGrid.Insert(new Tree(-24.3 + _random.NextDouble(), 31.1 + _random.NextDouble())));

            var exploreCount = 15;
            var explores = new List<Tree>(exploreCount);
            for (var i = 0; i < exploreCount; i++)
            {
                explores.Add(new Tree(-24.3 + _random.NextDouble(), 31.1 + _random.NextDouble()));
            }

            var sw = new Stopwatch();
            sw.Start();

            Parallel.ForEach(explores, t => _geoGrid.Explore(t.Gps, 9000, 2));
            sw.Stop();
            Console.WriteLine($"{exploreCount} Explores took {sw.ElapsedMilliseconds} ms.");
        }

        [Test]
        public void TestExploreAmount()
        {
            var treeCount = 10000;
            Parallel.For(0, treeCount,
                i => _geoGrid.Insert(new Tree(-24.3 + _random.NextDouble(), 31.1 + _random.NextDouble())));


            var res = _geoGrid.Explore(new Tree(-24.3 + _random.NextDouble(), 31.1 + _random.NextDouble()).Gps);
            Assert.IsTrue(res.Count() == treeCount);

        }


        [Test]
        //[Ignore("tooSlow")]
        public void TestExploreWorstCase()
        {
            var treeCount = 1400000;
            Parallel.For(0, treeCount,
                i => _geoGrid.Insert(new Tree(-24.3 + _random.NextDouble(), 31.1 + _random.NextDouble())));

            var exploreCount = 15;
            var explores = new List<Tree>(exploreCount);
            for (var i = 0; i < exploreCount; i++)
                explores.Add(new Tree(-24.3 + _random.NextDouble(), 31.1 + _random.NextDouble()));
            var sw = new Stopwatch();
            sw.Start();
            Parallel.ForEach(explores, t => _geoGrid.Explore(t.Gps, 9000, 1));
            sw.Stop();
            Console.WriteLine($"{exploreCount} Explores took {sw.ElapsedMilliseconds} ms.");
        }

        [Test]
        public void TestInsert()
        {
            var treeCount = 1400000;
            var trees = new List<Tree>(treeCount);


            for (var i = 0; i < treeCount; i++)
                trees.Add(new Tree(-24.3 + _random.NextDouble(), 31.1 + _random.NextDouble()));
            var sw = new Stopwatch();
            sw.Start();
            Parallel.ForEach(trees, t => _geoGrid.Insert(t));
            sw.Stop();
            Console.WriteLine($"Insert of {treeCount} trees took {sw.ElapsedMilliseconds} ms.");
        }


        [Test]
        public void TestMovement()
        {
            var treeCount = 15000;
            var trees = new List<Tree>(treeCount);


            for (var i = 0; i < treeCount; i++)
            {
                var tree = new Tree(-24.3 + _random.NextDouble(), 31.1 + _random.NextDouble());
                trees.Add(tree);
                _geoGrid.Insert(tree);
            }

            var movementCount = 24 * treeCount;
            var moves = new List<GeoCoordinate>(movementCount);
            for (var i = 0; i < movementCount; i++)
            {
                moves.Add(new GeoCoordinate(-24.3 + _random.NextDouble(), 31.1 + _random.NextDouble()));
            }

            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < 24; i++)
                Parallel.For
                (0, treeCount,
                    ti =>
                    {
                        var newT = _geoGrid.MoveToPosition(trees[ti], moves[ti * i].Latitude, moves[ti * i].Longitude);
                        Assert.AreEqual(newT.Latitude, moves[ti * i].Latitude);
                    });
            sw.Stop();
            Console.WriteLine($"{movementCount} Moves took {sw.ElapsedMilliseconds} ms.");
        }


        [Test]
        public void TestWrongIndex()
        {
            Assert.DoesNotThrow(() => _geoGrid.Explore(-25.517894615057646, 32.047639973401694, 1000, 3));
            Assert.DoesNotThrow(() => _geoGrid.Explore(-25.507404834383728, 32.04466243687046, 1000, 3));
        }
    }


    internal class Tree : IGeoCoordinate
    {
        public Tree(double lat, double lon)
        {
            TreeId = Guid.NewGuid();
            Gps = new GeoCoordinate(lat, lon);
        }

        public GeoCoordinate Gps { get; }

        public Guid TreeId { get; }

        public double Latitude
        {
            get { return Gps.Latitude; }
            set { Gps.Latitude = value; }
        }

        public double Longitude
        {
            get { return Gps.Longitude; }
            set { Gps.Longitude = value; }
        }

        /// <summary>
        ///   Position comparison for the IGeoCoordinate.
        /// </summary>
        /// <param name="other">The other X/Y coordinate pair.</param>
        /// <returns>'True', if both geo coordinates sufficiently close enough.</returns>
        public bool Equals(IGeoCoordinate other) {
            const double threshold = 0.00000000000001;
            return (Math.Abs(Latitude - other.Latitude) < threshold) &&
                   (Math.Abs(Longitude - other.Longitude) < threshold);
        }
    }
}