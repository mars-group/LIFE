using System;
using System.Diagnostics;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;
using NUnit.Framework;

namespace EnvironmentServiceComponentTests.SpatialApiTests
{
    public class CuboidTest
    {
        [Test]
        public void TestInner()
        {
            var b1 = new Cuboid(new Vector3(2, 2), Vector3.Zero);
            var b2 = new Cuboid(new Vector3(1, 1), Vector3.Zero);

            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestOuter()
        {
            var b1 = new Cuboid(new Vector3(2, 2), Vector3.Zero);
            var b2 = new Cuboid(new Vector3(4, 4), Vector3.Zero);
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestIntersectionByPart()
        {
            var b1 = new Cuboid(new Vector3(100, 100), new Vector3(50, 50));
            var b2 = new Cuboid(new Vector3(25, 25), new Vector3(1, 1));
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestTouching()
        {
            var b1 = new Cuboid(new Vector3(1, 1), Vector3.Zero);
            var b2 = new Cuboid(new Vector3(1, 1), new Vector3(2, 0));
            Assert.IsFalse(b1.IntersectsWith(b2));
            Assert.IsFalse(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestEquals()
        {
            var b1 = new Cuboid(new Vector3(2, 2), Vector3.Zero);
            var b2 = new Cuboid(new Vector3(2, 2), Vector3.Zero);
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestContains()
        {
            var b1 = new Cuboid(new Vector3(1, 1), Vector3.Zero);
            var b2 = new Cuboid(new Vector3(2, 2), Vector3.Zero);
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestPerformance()
        {
            var b1 = new Cuboid(Vector3.Random, Vector3.Random, new Direction());
            var b2 = new Cuboid(Vector3.Random, Vector3.Random, new Direction());
            var initTime = Stopwatch.StartNew();
            for (var i = 0; i < 1000000; i++) b1.IntersectsWith(b2);
            Console.WriteLine(initTime.ElapsedMilliseconds + " ms");
        }
    }
}