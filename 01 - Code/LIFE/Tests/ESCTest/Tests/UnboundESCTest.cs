using GenericAgentArchitectureCommon.Interfaces;
using SpatialCommon.Datatypes;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;

namespace ESCTest.Tests {
    using System;
    using System.Diagnostics;
    using System.Linq;
    using DalskiAgent.Environments;
    using DalskiAgent.Execution;
    using Entities;
    using ESCTestLayer.Entities;
    using ESCTestLayer.Implementation;
    using ESCTestLayer.Interface;
    using GeoAPI.Geometries;
    using NetTopologySuite.Geometries;
    using NUnit.Framework;

    public class UnboundESCTest {
        #region Setup / Tear down

        [SetUp]
        public void SetUp() {
            _esc = new UnboundESC();
        }


        [TearDown]
        public void TearDown() {}

        #endregion

        private IUnboundESC _esc;

        [Test]
        public void TestAddTwoAgentsAtSamePosition() {
            TestAgent2D a1 = new TestAgent2D(1, 1);
            TestAgent2D a2 = new TestAgent2D(1, 1);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.False(_esc.Add(a2, new TVector(0, 0)));
            Assert.False(_esc.Add(a2, new TVector(0.5d, 0.5d)));
            Assert.True(_esc.Add(a2, new TVector(1, 1)));
        }

        [Test]
        public void TestMoveAround()
        {
            TestAgent2D a1 = new TestAgent2D(2, 2);
            Assert.True(_esc.Add(a1, new TVector(1, 1)));
            Assert.True(_esc.Move(a1, new TVector(0, 1)).Success);
            Assert.True(a1.Geometry.Centroid.Equals(new Point(1, 2)));
            Assert.True(_esc.Move(a1, new TVector(0, -1)).Success);
            Assert.True(a1.Geometry.Centroid.Equals(new Point(1, 1)));
            Assert.True(_esc.Move(a1, new TVector(10, 0)).Success);
            Assert.True(a1.Geometry.Centroid.Equals(new Point(11, 1)));
            Assert.True(_esc.Move(a1, new TVector(-5, 0)).Success);
            Assert.True(a1.Geometry.Centroid.Equals(new Point(6, 1)));
            Assert.True(_esc.Move(a1, new TVector(-3.5d, 7.71d)).Success);
            Assert.True(a1.Geometry.Centroid.Equals(new Point(2.5d, 8.71d)));
        }

        [Test]
        public void TestMoveAgentForCollision()
        {
            TestAgent2D a1 = new TestAgent2D(1, 1);
            TestAgent2D a2 = new TestAgent2D(1, 1);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.True(_esc.Add(a2, new TVector(1, 1)));
            Assert.False(_esc.Move(a2, new TVector(-1, -1)).Success);
        }


        [Test]
        public void TestResize() {
            TestAgent2D a1 = new TestAgent2D(2, 2);
            TestAgent2D a2 = new TestAgent2D(2, 2);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.True(_esc.Add(a2, new TVector(3, 0)));

            IGeometry newGeometry = MyGeometryFactory.Rectangle(6, 2, a2.Geometry.Centroid.Coordinate);
            Assert.False(_esc.Resize(a2, newGeometry));
            newGeometry = MyGeometryFactory.Rectangle(4, 2, a2.Geometry.Centroid.Coordinate);
            Assert.True(_esc.Resize(a2, newGeometry));
        }

        [Test]
        public void TestAddWithRandomPositionToFillRoom() {
            for (int i = 0; i < 4; i++) {
                TestAgent2D a = new TestAgent2D(1, 1);
                Assert.True(_esc.AddWithRandomPosition(a, new TVector(0, 0), new TVector(1, 1), true));
            }

            Assert.True(_esc.ExploreAll().Count() == 4);

            TestAgent2D a2 = new TestAgent2D(1, 1);
            Assert.False(_esc.AddWithRandomPosition(a2, new TVector(0, 0), new TVector(1, 1), true));
        }


        [Test]
        public void TestManyAgentsWithRandomPosition() {
            for (int i = 0; i < 100; i++) {
                TestAgent2D a = new TestAgent2D(1, 1);
                Assert.True(_esc.AddWithRandomPosition(a, new TVector(0, 0), new TVector(100, 100), false));
            }
            for (int i = 0; i < 100; i++) {
                TestAgent2D a = new TestAgent2D(1, 1);
                Assert.True(_esc.AddWithRandomPosition(a, new TVector(0, 0), new TVector(100, 100), false));
            }
        }


        [Test]
        public void TestCorrectPlacement2D() {
            TestAgent2D a1 = new TestAgent2D(1, 1);
            TestAgent2D a2 = new TestAgent2D(1, 1);
            TestAgent2D a3 = new TestAgent2D(1, 1);
            TestAgent2D a4 = new TestAgent2D(1, 1);

            TVector pos = new TVector(1, 1, 0);
            Assert.True(_esc.Add(a1, pos));

            pos = new TVector(2, 1, 0);
            Assert.True(_esc.Add(a2, pos));

            pos = new TVector(2, 0, 0);
            Assert.True(_esc.Add(a3, pos));

            pos = new TVector(0, 2, 0);
            Assert.True(_esc.Add(a4, pos));
        }


        protected void PrintAllAgents() {
            Console.WriteLine(_esc.ExploreAll().Count() + " Agents found.");
            foreach (ISpatialEntity entity in _esc.ExploreAll()) {
   //             Console.WriteLine(entity.Geometry + " center: "+entity.Geometry.Centroid);
            }
            Console.WriteLine("---");
        }

        [Test]
        public void TestMoveAndCollideWithOneOtherAgent() {
            TestAgent2D a1 = new TestAgent2D(1, 1);
            TestAgent2D a2 = new TestAgent2D(1, 1);

            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.True(_esc.Add(a2, new TVector(0, 1)));

            IPoint oldCentroid = a2.Geometry.Centroid;
            MovementResult movementResult = _esc.Move(a2, new TVector(0, -1));
            Assert.False(movementResult.Success);
            Assert.True(movementResult.Collisions.Contains(a1));
            Assert.True(a2.Geometry.Centroid.Equals(oldCentroid));
        }


        [Test]
        public void TestRotation() {
            TestAgent2D a1 = new TestAgent2D(4, 2);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.False(a1.Geometry.Intersects(new Point(-0.9, -1.9)));
            Assert.False(a1.Geometry.Intersects(new Point(-0.9, 1.9)));
            Assert.False(a1.Geometry.Intersects(new Point(0.9, -1.9)));
            Assert.False(a1.Geometry.Intersects(new Point(0.9, 1.9)));

            Direction direction = new Direction();
            direction.SetYaw(90);
            var rotationVector = direction.GetDirectionalVector().GetTVector();
            Assert.True(_esc.Move(a1, TVector.Origin, rotationVector).Success);
            Assert.True(a1.Geometry.Intersects(new Point(-0.9, -1.9)));
            Assert.True(a1.Geometry.Intersects(new Point(-0.9, 1.9)));
            Assert.True(a1.Geometry.Intersects(new Point(0.9, -1.9)));
            Assert.True(a1.Geometry.Intersects(new Point(0.9, 1.9)));
        }

        [Test]
        public void TestAdd500Elements() {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 500; i++) {
                TestAgent2D a1 = new TestAgent2D(1, 1);
                Assert.True(_esc.Add(a1, new TVector(i, 0)));
            }
            // 4.9 sec für 5k agents.
            Console.WriteLine(stopwatch.ElapsedMilliseconds + " ms");
        }

        [Test]
        public void TestIntersections()
        {
            ESCAdapter adapter = new ESCAdapter(_esc, new Vector(1000, 1000), false);
            SeqExec exec = new SeqExec(false);

            var pos1 = new Vector(5d, 10.025d, 0d);
            var pos2 = new Vector(10.025d, 7.75d, 0);
            // The agents do not collide. They touch at point (10,10). Exception is thrown if ESC thinks there is a collision.
            var a1 = new TestSpatialAgent(exec, adapter, pos1, new Vector(10d, 0.05d, 0.4d), new Direction());
            var a2 = new TestSpatialAgent(exec, adapter, pos2, new Vector(0.05d, 4.5d, 0.4d), new Direction());

            Assert.True(a1.GetPosition().Equals(pos1));
            Assert.True(a2.GetPosition().Equals(pos2));
        }
    }
}