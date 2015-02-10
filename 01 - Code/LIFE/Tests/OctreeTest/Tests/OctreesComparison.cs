using System;
using System.Collections.Generic;
using NUnit.Framework;
using OctreeTest.Entity;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Shape;
using SpatialObjectOctree.Implementation;

namespace OctreeTest.Tests {

    public class OctreesComparison {
        private const double Tolerance = 0.00001;
        private const int Amount = 20000;
        private Octree<TestEntity> _octreeNew;
        private SpatialObjectOctree<TestEntity> _octreeOld;
        private Quadtree _quadTree;

        [Test]
        public void TestOctree() {
            _octreeNew = new Octree<TestEntity>(0, new Vector3(0, 0), new Vector3(1000, 1000));

            for (int i = 0; i < Amount; i++) {
                TestEntity m1 = new TestEntity(BoundingBox.GenerateByDimension(new Vector3(1, 1), new Vector3(1, 1)));
                _octreeNew.Insert(m1);
            }
        }

        [Test]
        public void TestQuadTreeFlo() {
            _quadTree = new Quadtree(0, new Float2(0, 0), new Float2(1000, 1000));
            _octreeNew = new Octree<TestEntity>(0, new Vector3(0, 0), new Vector3(1000, 1000));
            _octreeOld = new SpatialObjectOctree<TestEntity>(new Vector3(25, 25, 25), 1, true);

            TestEntity m1 = new TestEntity(BoundingBox.GenerateByDimension(new Vector3(1, 1), new Vector3(1, 1)));
            TestEntity query = new TestEntity
                (BoundingBox.GenerateByDimension(new Vector3(50, 50), new Vector3(100, 100)));
            Console.WriteLine(m1.Bounds);
            Console.WriteLine(query.Bounds);
            Console.WriteLine();

            _quadTree.Insert(m1);
            _octreeNew.Insert(m1);
            _octreeOld.Insert(m1);

            var resultQuadtree = _quadTree.Retrieve
                (new List<ISpatialEntity>(),
                    new Float2((float) query.Bounds.Position.X, (float) query.Bounds.Position.Y),
                    new Float2((float) query.Bounds.Dimension.X, (float) query.Bounds.Dimension.Y));
            List<TestEntity> resultNew = _octreeNew.Query(query.Bounds);
            List<TestEntity> resultOld = _octreeOld.Query(query.Bounds);

            Console.WriteLine("resultQuadtree.Count " + resultQuadtree.Count);
            Console.WriteLine("resultNew.Count " + resultNew.Count);
            Console.WriteLine("resultOld.Count " + resultOld.Count);

            Assert.IsTrue(m1.Bounds.IntersectsWith(query.Bounds));
            Assert.IsTrue(resultOld.Count.Equals(resultNew.Count));
        }

        [Test]
        public void TestQuadTreeComparison() {
            _octreeNew = new Octree<TestEntity>(0, new Vector3(0, 0), new Vector3(1000, 1000));
            _octreeOld = new SpatialObjectOctree<TestEntity>(new Vector3(25, 25, 25), 1, true);

            for (int i = 0; i < Amount; i++) {
                TestEntity m1 = new TestEntity(BoundingBox.GenerateByDimension(new Vector3(i, i), new Vector3(1, 1)));
                _octreeNew.Insert(m1);
                _octreeOld.Insert(m1);
            }
            Assert.IsTrue(Amount == _octreeNew.GetAll().Count);
            Assert.IsTrue(Amount == ((SpatialObjectOctree<TestEntity>) _octreeOld).GetShapeCount());

            TestEntity query = new TestEntity
                (BoundingBox.GenerateByDimension(new Vector3(105, 105), new Vector3(10, 10)));
            List<TestEntity> resultQuadTree = _octreeNew.Query(query.Bounds);
            Console.WriteLine(resultQuadTree.Count);
            List<TestEntity> resultFlo = _octreeOld.Query(query.Bounds);
            Console.WriteLine(resultFlo.Count);
            Assert.IsTrue(resultQuadTree.Count.Equals(resultFlo.Count));
        }
    }

}