using System;
using System.Diagnostics;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;
using NUnit.Framework;

namespace EnvironmentServiceComponentTests.SpatialApiTests
{
    public class BoundingBoxTest
    {
        [Test]
        public void TestTransformWithZeroMovement()
        {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 10, -0.2), new Vector3(10, 10.5, 0.2));
            var transformed = b1.Transform(Vector3.Zero, new Direction());
            Assert.IsTrue(b1.Position.Equals(transformed.Position));
        }

        [Test]
        public void TestTransform()
        {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 10, -0.2), new Vector3(10, 10.5, 0.2));
            var transformed = b1.Transform(Vector3.Right, new Direction());
            Assert.IsTrue(transformed.Position.Equals(b1.Position + Vector3.Right));

            var movementVector = Vector3.Random;
            transformed = b1.Transform(movementVector, new Direction());
            Assert.IsTrue(transformed.Position.Equals(b1.Position + movementVector));
        }

        [Test]
        public void TestBoundingBoxPerformance()
        {
            var b1 = BoundingBox.GenerateByCorners(Vector3.Random, Vector3.Random);
            var b2 = BoundingBox.GenerateByCorners(Vector3.Random, Vector3.Random);
            var initTime = Stopwatch.StartNew();
            for (var i = 0; i < 1000000; i++) b1.IntersectsWith(b2);
            Console.WriteLine(initTime.ElapsedMilliseconds + " ms");
        }

        [Test]
        public void TestInner()
        {
            var b1 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(1, 1));
            Assert.IsTrue(b1.IntersectsWith((IShape) b2));
            Assert.IsTrue(b2.IntersectsWith((IShape) b1));
        }

        [Test]
        public void TestOuter()
        {
            var b1 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(4, 4));
            Assert.IsTrue(b1.IntersectsWith((IShape) b2));
            Assert.IsTrue(b2.IntersectsWith((IShape) b1));
        }

        [Test]
        public void TestIntersectionByPart()
        {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(100, 100));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(-11.5, -11.5), new Vector3(13.5, 13.5));
            Assert.IsTrue(b1.IntersectsWith((IShape) b2));
            Assert.IsTrue(b2.IntersectsWith((IShape) b1));
        }

        [Test]
        public void TestTouching()
        {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(2, 0), new Vector3(4, 2));
            Assert.IsFalse(b1.IntersectsWith((IShape) b2));
            Assert.IsFalse(b2.IntersectsWith((IShape) b1));
        }

        [Test]
        public void TestNoIntersection()
        {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(2, 2.1), new Vector3(4, 4));
            Assert.IsFalse(b1.IntersectsWith((IShape) b2));
            Assert.IsFalse(b2.IntersectsWith((IShape) b1));
        }

        [Test]
        public void TestEquals()
        {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            Assert.IsTrue(b1.IntersectsWith((IShape) b2));
            Assert.IsTrue(b2.IntersectsWith((IShape) b1));
        }

        [Test]
        public void TestContains()
        {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0.5, 0.5), new Vector3(1.5, 1.5));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(-23.5, -23.5), new Vector3(1.5, 1.5));
            Assert.IsTrue(b1.IntersectsWith((IShape) b2));
            Assert.IsTrue(b2.IntersectsWith((IShape) b1));
        }
    }
}