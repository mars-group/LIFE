using System;
using System.Linq;
using System.Windows;
using CSharpQuadTree;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using EnvironmentServiceComponent.Entities.Shape;
using EnvironmentServiceComponent.Implementation;
using ESCTest.Entities;
using NUnit.Framework;
using SpatialCommon.Enums;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;
using Direction = SpatialCommon.Datatypes.Direction;
using Vector = SpatialCommon.Datatypes.Vector;

namespace ESCTest.Tests {

    public class TestRectESC : TestESC {
        #region Setup / Tear down

        [SetUp]
        public void SetUp() {
            _esc = new RectESC();
        }


        [TearDown]
        public void TearDown() {}

        #endregion

        protected override ISpatialEntity GenerateAgent(double x, double y) {
            return GenerateAgent(x, y, CollisionType.MassiveAgent);
        }

        protected override ISpatialEntity GenerateAgent(double x, double y, CollisionType collisionType) {
            return new RectAgent(x, y, collisionType);
        }

       

        [Test]
        public void TestIntersections() {
            ESCRectAdapter adapter = new ESCRectAdapter(_esc, new Vector(1000, 1000), false);
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
        public void TestSpatialAgentPlacementAboveAndBelowObstacle() {
            ESCRectAdapter adapter = new ESCRectAdapter(_esc, new Vector(1000, 1000), false);
            SeqExec exec = new SeqExec(false);

            // centerY = 10
            Vector posObstacle = new Vector(10d, 10d, 0d);
            // minY = 10 - 0.025 = 9.975, maxY = 10 + 0.025 = 10.025
            Vector dimObstacle = new Vector(10d, 0.05d, 0.4d);

            // centerY = 9.75
            Vector posPedestrianAbove = new Vector(10d, 9.75d, 0d);
            // minY = 9.75 - 0.2 = 9.55, maxY = 9.75 + 0.2 = 9.95
            Vector dimPedestrianAbove = new Vector(0.4d, 0.4d, 0.4d);
            // pedestrian1 is 0.025 above obstacle (not touching)

            // centerY = 10.25
            Vector posPedestrianBelow = new Vector(10d, 10.25d, 0d);
            // minY = 10.25 - 0.2 = 10.05, maxY = 10.25 + 0.2 = 10.45
            Vector dimPedestrianBelow = new Vector(0.4d, 0.4d, 0.4d);
            // pedestrian1 is 0.025 below obstacle (not touching)

            new TestSpatialAgent(exec, adapter, posObstacle, dimObstacle, new Direction());
            PrintAllAgents();
            new TestSpatialAgent(exec, adapter, posPedestrianBelow, dimPedestrianBelow, new Direction());
            PrintAllAgents();
            new TestSpatialAgent(exec, adapter, posPedestrianAbove, dimPedestrianAbove, new Direction());
            PrintAllAgents();
        }

        [Test]
        public void TestMoveTwoAgentsOnSamePosition() {
            ISpatialEntity a1 = GenerateAgent(10, 10);
            ISpatialEntity a2 = GenerateAgent(10, 10);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.True(_esc.Add(a2, new TVector(15, 15)));
            Assert.False(_esc.Move(a1, new TVector(15, 15)).Success);
            Assert.True(_esc.Move(a2, new TVector(30, 30)).Success);
            Assert.True(_esc.Move(a1, new TVector(15, 15)).Success);

            PrintAllAgents();
        }

        protected void PrintAllAgents() {
            Console.WriteLine(_esc.ExploreAll().Count() + " Agents found.");
            foreach (ISpatialEntity entity in _esc.ExploreAll()) {
                RectShape rectShape = entity.Shape as RectShape;
                Rect bounds = rectShape.Bounds;
                Console.WriteLine
                    (entity + " " + rectShape.GetPosition() + " // " + bounds.TopLeft + " -> " + bounds.BottomRight);
            }
            Console.WriteLine("---");
        }
    }

}