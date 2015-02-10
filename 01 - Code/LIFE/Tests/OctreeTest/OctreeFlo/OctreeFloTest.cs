using System;
using System.Collections.Generic;
using System.Windows;
using CSharpQuadTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OctreeFlo.Implementation;
using OctreeFlo.Interface;
using SpatialCommon.Shape;
using SpatialCommon.Transformation;

namespace OctreeTest.OctreeFlo {

    [TestClass]
    public class OctreeFloTest {
        private const double Tolerance = 0.00001;
        private const int Amount = 100000;
        private QuadTree<ChimeraQuadObject> _quadTree;
        private IOctreeFlo<ChimeraQuadObject> _octree;

        [TestMethod]
        public void TestChimeraQuadObjectDuality() {
            BoundingBox box = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(4, 2));
            ChimeraQuadObject m1 = new ChimeraQuadObject(box.LeftBottomFront, box.RightTopRear);
            Assert.IsTrue(Math.Abs(m1.BoundsRect.Width - m1.Bounds.Dimension.X) < Tolerance);
            Assert.IsTrue(Math.Abs(m1.BoundsRect.Height - m1.Bounds.Dimension.Y) < Tolerance);

            Assert.IsTrue(Math.Abs(m1.BoundsRect.X - m1.Bounds.LeftBottomFront.X) < Tolerance);
            Assert.IsTrue(Math.Abs(m1.BoundsRect.Y - m1.Bounds.LeftBottomFront.Y) < Tolerance);
        }

        [TestMethod]
        public void TestChimeraQuadObjectIntersection() {
            ChimeraQuadObject m1 = ChimeraQuadObject.ByDim(new Vector3(0, 0), new Vector3(2, 2));
            ChimeraQuadObject m2 = ChimeraQuadObject.ByDim(new Vector3(0, 0), new Vector3(2, 2));
            Assert.IsTrue(m1.BoundsRect.IntersectsWith(m2.BoundsRect));
            Assert.IsTrue(m1.Bounds.Intersects(m2.Bounds));
        }

        [TestMethod]
        public void TestQuadTreeFlo()
        {
            _quadTree = new QuadTree<ChimeraQuadObject>(new Size(25, 25), 1, true);
            _octree = new OctreeFlo<ChimeraQuadObject>(new Vector3(25, 25,25), 1, true);

            ChimeraQuadObject m1 = ChimeraQuadObject.ByDim(new Vector3(1, 1), new Vector3(1, 1));
            ChimeraQuadObject query = ChimeraQuadObject.ByDim(new Vector3(50, 50), new Vector3(100, 100));
            Console.WriteLine(m1.Bounds);
            Console.WriteLine(query.Bounds);
            Console.WriteLine();

            _quadTree.Insert(m1);
            _octree.Insert(m1);

            List<ChimeraQuadObject> resultRect = _quadTree.Query(query.BoundsRect);
            List<ChimeraQuadObject> resultFlo = _octree.Query(query.Bounds);

            Console.WriteLine("resultRect.Count "+resultRect.Count);
            Console.WriteLine("resultFlo.Count " + resultFlo.Count);

            Assert.IsTrue(resultFlo.Count.Equals(resultRect.Count));
        }


        [TestMethod]
        public void TestQuadTreeComparison()
        {
            _quadTree = new QuadTree<ChimeraQuadObject>(new Size(25, 25), 1, true);
            _octree = new OctreeFlo<ChimeraQuadObject>(new Vector3(25, 25, 25), 1, true);

            for (int i = 0; i < Amount; i++)
            {
                ChimeraQuadObject m1 = ChimeraQuadObject.ByDim(new Vector3(i, i), new Vector3(1, 1));
                _quadTree.Insert(m1);
                _octree.Insert(m1);
            }
            Assert.IsTrue(Amount == _quadTree.GetQuadObjectCount());
            Assert.IsTrue(Amount == _octree.GetShapeCount());

            ChimeraQuadObject query = ChimeraQuadObject.ByDim(new Vector3(105, 105), new Vector3(10, 10));
            List<ChimeraQuadObject> resultQuadTree = _quadTree.Query(query.BoundsRect);
            Console.WriteLine(resultQuadTree.Count);
            List<ChimeraQuadObject> resultFlo = _octree.Query(query.Bounds);
            Console.WriteLine(resultFlo.Count);
            Assert.IsTrue(resultQuadTree.Count.Equals(resultFlo.Count));
        }
        
    }

}