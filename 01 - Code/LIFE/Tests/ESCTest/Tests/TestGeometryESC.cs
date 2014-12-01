using System;
using System.Linq;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using EnvironmentServiceComponent.Entities;
using EnvironmentServiceComponent.Entities.Shape;
using EnvironmentServiceComponent.Implementation;
using ESCTest.Entities;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using SpatialCommon.Datatypes;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;

namespace ESCTest.Tests {

    public class TestGeometryESC : TestESC {
        #region Setup / Tear down

        [SetUp]
        public void SetUp() {
            _esc = new GeometryESC();
        }


        [TearDown]
        public void TearDown() {}

        #endregion

        [Test]
        public void TestIntersections() {
            ESCAdapter adapter = new ESCAdapter(_esc, new Vector(1000, 1000), false);
            SeqExec exec = new SeqExec(false);

            Vector pos1 = new Vector(5d, 10.025d, 0d);
            Vector pos2 = new Vector(10.026d, 7.76d, 0d);
            // The agents do not collide. They touch at point (10,10). Exception is thrown if ESC thinks there is a collision.
            TestSpatialAgent a1 = new TestSpatialAgent
                (exec, adapter, pos1, new Vector(10d, 0.05d, 0.4d), new Direction());
            TestSpatialAgent a2 = new TestSpatialAgent
                (exec, adapter, pos2, new Vector(0.05d, 4.5d, 0.4d), new Direction());

            Assert.True(a1.GetPosition().Equals(pos1));
            Assert.True(a2.GetPosition().Equals(pos2));
        }

        [Test]
        public void TestMoveAndCollideWithOneOtherAgent()
        {
            var a1 = GenerateAgent(0.9, 0.9);
            var a2 = GenerateAgent(0.9, 0.9);

            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.True(_esc.Add(a2, new TVector(0, 1)));

            IPoint oldCentroid = (a2.Shape as GeometryShape).Geometry.Centroid;
            MovementResult movementResult = _esc.Move(a2, new TVector(0, -1));
            Assert.False(movementResult.Success);
            Assert.True(movementResult.Collisions.Contains(a1));
            Assert.True((a2.Shape as GeometryShape).Geometry.Centroid.Equals(oldCentroid));
        }


        [Test]
        public void TestMoveAround()
        {
            var a1 = new GeometryAgent(2, 2);
            Assert.True(_esc.Add(a1, new TVector(1, 1)));
            Assert.True(_esc.Move(a1, new TVector(0, 1)).Success);
            Assert.True((a1.Shape as GeometryShape).Geometry.Centroid.Equals(new Point(1, 2)));
            Assert.True(_esc.Move(a1, new TVector(0, -1)).Success);
            Assert.True((a1.Shape as GeometryShape).Geometry.Centroid.Equals(new Point(1, 1)));
            Assert.True(_esc.Move(a1, new TVector(10, 0)).Success);
            Assert.True((a1.Shape as GeometryShape).Geometry.Centroid.Equals(new Point(11, 1)));
            Assert.True(_esc.Move(a1, new TVector(-5, 0)).Success);
            Assert.True((a1.Shape as GeometryShape).Geometry.Centroid.Equals(new Point(6, 1)));
            Assert.True(_esc.Move(a1, new TVector(-3.5d, 7.71d)).Success);
            Assert.True((a1.Shape as GeometryShape).Geometry.Centroid.Equals(new Point(2.5d, 8.71d)));
        }

        protected void PrintAllAgents()
        {
            Console.WriteLine(_esc.ExploreAll().Count() + " Agents found.");
            foreach (ISpatialEntity entity in _esc.ExploreAll())
            {
                var geometryShape = entity.Shape as GeometryShape;
                Console.WriteLine(entity + " " + geometryShape.Geometry.Envelope);
            }
            Console.WriteLine("---");
        }

        [Test]
        public void TestResize()
        {
            GeometryAgent a1 = new GeometryAgent(2, 2);
            GeometryAgent a2 = new GeometryAgent(2, 2);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.True(_esc.Add(a2, new TVector(3.1, 0)));

            TVector pos1 = a1.Shape.GetPosition();
            IGeometry newGeometry = MyGeometryFactory.Rectangle(6, 2, new Coordinate(pos1.X, pos1.Y));
            Assert.False(_esc.Resize(a2, new ExploreShape(newGeometry)));
            PrintAllAgents();
            TVector pos2 = a2.Shape.GetPosition();
            newGeometry = MyGeometryFactory.Rectangle(4, 2, new Coordinate(pos2.X, pos2.Y));
            Assert.True(_esc.Resize(a2, new ExploreShape(newGeometry)));
        }

        [Test]
        public void TestRotation()
        {
            GeometryAgent a1 = new GeometryAgent(4, 2);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.False((a1.Shape as GeometryShape).Geometry.Intersects(new Point(-0.9, -1.9)));
            Assert.False((a1.Shape as GeometryShape).Geometry.Intersects(new Point(-0.9, 1.9)));
            Assert.False((a1.Shape as GeometryShape).Geometry.Intersects(new Point(0.9, -1.9)));
            Assert.False((a1.Shape as GeometryShape).Geometry.Intersects(new Point(0.9, 1.9)));

            Direction direction = new Direction();
            direction.SetYaw(90);
            TVector rotationVector = direction.GetDirectionalVector().GetTVector();
            Assert.True(_esc.Move(a1, TVector.Origin, rotationVector).Success);
            Assert.True((a1.Shape as GeometryShape).Geometry.Intersects(new Point(-0.9, -1.9)));
            Assert.True((a1.Shape as GeometryShape).Geometry.Intersects(new Point(-0.9, 1.9)));
            Assert.True((a1.Shape as GeometryShape).Geometry.Intersects(new Point(0.9, -1.9)));
            Assert.True((a1.Shape as GeometryShape).Geometry.Intersects(new Point(0.9, 1.9)));
        }

        protected override ISpatialEntity GenerateAgent(double x, double y)
        {
            return new GeometryAgent(x, y);
        }
    }

}