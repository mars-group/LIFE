using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OctreeFlo.Implementation;
using SpatialCommon.Shape;
using SpatialCommon.Transformation;

namespace OctreeTest.OctreeFlo {

    [TestClass]
    public class OctreeVsBoundingBoxTest {
        private readonly OctreeFlo<BoundingBox> _octree = new OctreeFlo<BoundingBox>(new Vector3(25, 25, 25), 1, true);
        private readonly Random _random = new Random();

        private Vector3 Random() {
            return new Vector3(_random.Next(100), _random.Next(100), _random.Next(100));
        }

        private void TestIntersection(BoundingBox b1, BoundingBox b2) {
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));

            _octree.Insert(b1);
            Assert.IsTrue(_octree.Query(b2).First().Equals(b1));
            _octree.Remove(b1);

            _octree.Insert(b2);
            Assert.IsTrue(_octree.Query(b1).First().Equals(b2));
            _octree.Remove(b2);
        }

        [TestMethod]
        public void TestBoundingBoxPerformance() {
            var dimension = new Vector3(10, 10, 10);

            for (var i = 0; i < 100000; i++) {
                var b1 = BoundingBox.GenerateByDimension(Random(), dimension);
                var b2 = BoundingBox.GenerateByDimension(Random(), dimension);

                _octree.Insert(b1);

                if (b1.Intersects(b2) != (_octree.Query(b2).Count == 1)) {
                    Console.WriteLine(b1);
                    Console.WriteLine(b2);
                    Console.WriteLine(b1.Intersects(b2));
                    Console.WriteLine(_octree.Query(b2).Count);
                    Console.WriteLine(_octree.Query(b2).First());
                }

                Assert.IsTrue(b1.Intersects(b2) == (_octree.Query(b2).Count == 1));

                _octree.Remove(b1);
            }
        }

        [TestMethod]
        public void TestAny() {
            //BoundingBox(( 9,00|49,00|62,00)->(19,00|59,00|72,00))
            //BoundingBox(( 4,00|42,00|58,00)->(14,00|52,00|68,00))
            var b1 = BoundingBox.GenerateByCorners(new Vector3(9, 49, 62), new Vector3(19, 59, 72));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(4, 42, 58), new Vector3(14, 52, 68));
            TestIntersection(b1, b2);
        }

        [TestMethod]
        public void TestInner() {
            var b1 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(1, 1));
            TestIntersection(b1, b2);
        }

        [TestMethod]
        public void TestOuter() {
            var b1 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(4, 4));
            TestIntersection(b1, b2);
        }

        [TestMethod]
        public void TestIntersectionByPart() {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(100, 100));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(-11.5, -11.5), new Vector3(13.5, 13.5));
            TestIntersection(b1, b2);
        }

        [TestMethod]
        public void TestTouching() {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(2, 0), new Vector3(4, 2));
            
            Assert.IsFalse(b1.IntersectsWith(b2));
            Assert.IsFalse(b2.IntersectsWith(b1));

            _octree.Insert(b1);
            Assert.IsFalse(_octree.Query(b2).Any());
            _octree.Remove(b1);

            _octree.Insert(b2);
            Assert.IsFalse(_octree.Query(b1).Any());
            _octree.Remove(b2);
        }

        [TestMethod]
        public void TestEquals() {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            TestIntersection(b1, b2);
        }

        [TestMethod]
        public void TestContains() {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0.5, 0.5), new Vector3(1.5, 1.5));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(-23.5, -23.5), new Vector3(1.5, 1.5));
            TestIntersection(b1, b2);
        }
    }

}