using System;
using System.Diagnostics;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;
using NUnit.Framework;

namespace EnvironmentServiceComponentTests.SpatialApiTests
{
    public class SphereTest
    {
        [Test]
        public void TestTransformWithZeroMovement()
        {
            var b1 = new Sphere(Vector3.Random, 10);
            var transformed = b1.Transform(Vector3.Zero, new Direction());
            Assert.IsTrue(b1.Position.Equals(transformed.Position));
        }

        [Test]
        public void TestTransform()
        {
            var b1 = new Sphere(Vector3.Random, 10);
            var transformed = b1.Transform(Vector3.Right, new Direction());
            Assert.IsTrue(transformed.Position.Equals(b1.Position + Vector3.Right));

            var movementVector = Vector3.Random;
            transformed = b1.Transform(movementVector, new Direction());
            Assert.IsTrue(transformed.Position.Equals(b1.Position + movementVector));
        }

        [Test]
        public void TestPerformance()
        {
            var b1 = new Sphere(Vector3.Random, 10);
            var b2 = new Sphere(Vector3.Random, 10);
            var initTime = Stopwatch.StartNew();

            for (var i = 0; i < 1000000; i++) b1.IntersectsWith(b2);
            Console.WriteLine(initTime.ElapsedMilliseconds + " ms");
        }

        [Test]
        public void TestInner()
        {
            var b1 = new Sphere(Vector3.Zero, 10);
            var b2 = new Sphere(Vector3.Zero, 5);
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestOuter()
        {
            var b1 = new Sphere(Vector3.Zero, 10);
            var b2 = new Sphere(new Vector3(3, 3, 3), 5);
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestIntersectionByPart()
        {
            var b1 = new Sphere(new Vector3(9, 9, 9), 4);
            var b2 = new Sphere(new Vector3(10, 10, 10), 4);
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestTouching()
        {
            var b1 = new Sphere(Vector3.Zero, 0.5);
            var b2 = new Sphere(Vector3.Up, 0.5);
            Assert.IsFalse(b1.IntersectsWith(b2));
            Assert.IsFalse(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestNoIntersection()
        {
            var b1 = new Sphere(new Vector3(6, 6, 6), 1);
            var b2 = new Sphere(new Vector3(10, 10, 10), 1);
            Assert.IsFalse(b1.IntersectsWith(b2));
            Assert.IsFalse(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestEquals()
        {
            var b1 = new Sphere(Vector3.Zero, 10);
            var b2 = new Sphere(Vector3.Zero, 10);
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }
    }
}