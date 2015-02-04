using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Shape;

namespace SpatialTests {

    [TestClass]
    public class BoundingBoxTest {
        private const double Epsilon = 0.00001;


        [TestMethod]
        public void TestBoundingBoxPerformance() {
            BoundingBox b1 = BoundingBox.GenerateByCorners(Vector3.Random, Vector3.Random);
            BoundingBox b2 = BoundingBox.GenerateByCorners(Vector3.Random, Vector3.Random);
            Stopwatch initTime = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++) {
                b1.Intersects(b2);
            }
            Console.WriteLine(initTime.ElapsedMilliseconds + " ms");
        }

        [TestMethod]
        public void TestQuadPerformance() {
            Cuboid b1 = new Cuboid(Vector3.Random, Vector3.Random, new Direction());
            Cuboid b2 = new Cuboid(Vector3.Random, Vector3.Random, new Direction());
            Stopwatch initTime = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++) {
                b1.IntersectsWith(b2);
            }
            Console.WriteLine(initTime.ElapsedMilliseconds + " ms");
        }


        [TestMethod]
        public void TestInner() {
            BoundingBox b1 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(2, 2));
            BoundingBox b2 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(1, 1));
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [TestMethod]
        public void TestOuter() {
            BoundingBox b1 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(2, 2));
            BoundingBox b2 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(4, 4));
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [TestMethod]
        public void TestIntersectionByPart() {
            BoundingBox b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(100,100));
            BoundingBox b2 = BoundingBox.GenerateByCorners(new Vector3(-11.5, -11.5), new Vector3(13.5, 13.5));
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [TestMethod]
        public void TestTouching() {
            BoundingBox b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            BoundingBox b2 = BoundingBox.GenerateByCorners(new Vector3(2, 0), new Vector3(4, 2));
            Assert.IsFalse(b1.IntersectsWith(b2));
            Assert.IsFalse(b2.IntersectsWith(b1));
        }

        [TestMethod]
        public void TestEquals() {
            BoundingBox b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            BoundingBox b2 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [TestMethod]
        public void TestContains() {
            BoundingBox b1 = BoundingBox.GenerateByCorners(new Vector3(0.5, 0.5), new Vector3(1.5, 1.5));
            BoundingBox b2 = BoundingBox.GenerateByCorners(new Vector3(-23.5, -23.5), new Vector3(1.5, 1.5));
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

    }

}