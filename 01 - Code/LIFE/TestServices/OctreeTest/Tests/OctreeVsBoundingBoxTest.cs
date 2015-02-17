using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using OctreeTest.Entity;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Shape;
using SpatialObjectOctree.Implementation;

namespace OctreeTest.Tests {

    public class OctreeVsBoundingBoxTest {
        private readonly Random _random = new Random();
        private SpatialObjectOctree<TestEntity> _octree;

        private Vector3 Random() {
            return new Vector3(_random.Next(100), _random.Next(100), _random.Next(100));
        }

        [Test] // Do a number of insertion tests to measure general performance.
        public void PerformanceTest() {
            // Test series.
            int[] tests = {1000, 2000, 4000, 8000, 16000, 32000, 64000, 128000};
            const int envWidth = 1000; // Important: Ensure that width*height
            const int envHeight = 1000; // is sufficient to store max. number of agents!

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Perform tests and output elapsed time.
            for (int t = 0; t < tests.Length; t++) {
                _octree = new SpatialObjectOctree<TestEntity>
                    (new Vector3(25, 25, 25), 1, true);
                for (int i = 0; i < tests[t]; i++) {
                    var b1 = BoundingBox.GenerateByDimension
                        (new Vector3(i%envWidth, i/envWidth), new Vector3(0.9, 0.9, 0.9));
                    var t1 = new TestEntity(b1);
                    _octree.Insert(t1);
                }
                Console.WriteLine("[" + tests[t] + " agents]: " + stopwatch.ElapsedMilliseconds + " ms");
                stopwatch.Restart(); // Reset stopwatch.
            }
        }

        private void TestIntersection(TestEntity t1, TestEntity t2) {
            var b1 = t1.Shape.Bounds;
            var b2 = t2.Shape.Bounds;
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));

            _octree.Insert(t1);
            Assert.IsTrue(_octree.Query(b2).First().Shape.Bounds.Equals(b1));
            _octree.Remove(t1);

            _octree.Insert(t2);
            Assert.IsTrue(_octree.Query(b1).First().Shape.Bounds.Equals(b2));
            _octree.Remove(t2);
        }

        [Test]
        public void TestOctreeExpand() {
            _octree = new SpatialObjectOctree<TestEntity>
                (new Vector3(25, 25, 25), 1, true);
            var b1 = BoundingBox.GenerateByCorners(new Vector3(11, 48, 29), new Vector3(21, 58, 29));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(12, 10, 80), new Vector3(22, 20, 80));
            _octree.Insert(new TestEntity(b1));
            _octree.Insert(new TestEntity(b2));
        }

        [Test]
        public void TestQueryAsIntersects() {
            _octree = new SpatialObjectOctree<TestEntity>
                (new Vector3(25, 25, 25), 1, true);
            var dimension = new Vector3(10, 10, 10);

            for (var i = 0; i < 100000; i++) {
                var b1 = BoundingBox.GenerateByDimension(Random(), dimension);
                var b2 = BoundingBox.GenerateByDimension(Random(), dimension);

                var t1 = new TestEntity(b1);
                _octree.Insert(t1);
                Assert.IsTrue(b1.IntersectsWith(b2) == (_octree.Query(b2).Count == 1));
                _octree.Remove(t1);
            }
        }

        [Test]
        public void TestAny() {
            _octree = new SpatialObjectOctree<TestEntity>
                (new Vector3(25, 25, 25), 1, true);
            //BoundingBox(( 9,00|49,00|62,00)->(19,00|59,00|72,00))
            //BoundingBox(( 4,00|42,00|58,00)->(14,00|52,00|68,00))
            var b1 = BoundingBox.GenerateByCorners(new Vector3(9, 49, 62), new Vector3(19, 59, 72));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(4, 42, 58), new Vector3(14, 52, 68));
            TestIntersection(new TestEntity(b1), new TestEntity(b2));
        }

        [Test]
        public void TestInner() {
            _octree = new SpatialObjectOctree<TestEntity>
                (new Vector3(25, 25, 25), 1, true);
            var b1 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(1, 1));
            TestIntersection(new TestEntity(b1), new TestEntity(b2));
        }

        [Test]
        public void TestOuter() {
            _octree = new SpatialObjectOctree<TestEntity>
                (new Vector3(25, 25, 25), 1, true);
            var b1 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(4, 4));
            TestIntersection(new TestEntity(b1), new TestEntity(b2));
        }

        [Test]
        public void TestIntersectionByPart() {
            _octree = new SpatialObjectOctree<TestEntity>
                (new Vector3(25, 25, 25), 1, true);
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(100, 100));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(-11.5, -11.5), new Vector3(13.5, 13.5));
            TestIntersection(new TestEntity(b1), new TestEntity(b2));
        }

        [Test]
        public void TestTouching() {
            _octree = new SpatialObjectOctree<TestEntity>
                (new Vector3(25, 25, 25), 1, true);
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(2, 0), new Vector3(4, 2));

            Assert.IsFalse(b1.IntersectsWith(b2));
            Assert.IsFalse(b2.IntersectsWith(b1));

            var t1 = new TestEntity(b1);
            _octree.Insert(t1);
            Assert.IsFalse(_octree.Query(b2).Any());
            _octree.Remove(t1);

            var t2 = new TestEntity(b2);
            _octree.Insert(t2);
            Assert.IsFalse(_octree.Query(b1).Any());
            _octree.Remove(t2);
        }

        [Test]
        public void TestEquals() {
            _octree = new SpatialObjectOctree<TestEntity>
                (new Vector3(25, 25, 25), 1, true);
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            TestIntersection(new TestEntity(b1), new TestEntity(b2));
        }

        [Test]
        public void TestContains() {
            _octree = new SpatialObjectOctree<TestEntity>
                (new Vector3(25, 25, 25), 1, true);
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0.5, 0.5), new Vector3(1.5, 1.5));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(-23.5, -23.5), new Vector3(1.5, 1.5));
            TestIntersection(new TestEntity(b1), new TestEntity(b2));
        }
    }

}