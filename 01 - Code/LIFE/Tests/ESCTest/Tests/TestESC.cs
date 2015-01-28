using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EnvironmentServiceComponent.Implementation;
using ESCTest.Entities;
using LifeAPI.Environment;
using LifeAPI.Spatial;
using NUnit.Framework;
using SpatialCommon.Transformation;

namespace ESCTest.Tests {

    public class TestESC {
        private IEnvironment _esc = new EnviromentServiceComponent();

        [Test]
        public void TestAddFirstAgent() {
            ISpatialEntity a1 = GenerateAgent(1, 1);
            Assert.True(_esc.Add(a1, Vector3.Zero));
        }

        [Test]
        public void TestAddTwoAgentsAtAdjacentPositions() {
            ISpatialEntity a1 = GenerateAgent(1, 1);
            ISpatialEntity a2 = GenerateAgent(1, 1);
            Assert.True(_esc.Add(a1, new Vector3(0, 0)));
            Assert.False(_esc.Add(a2, new Vector3(0.99, 0)));
            Assert.True(_esc.Add(a2, new Vector3(1, 0)));
        }

        [Test]
        public void TestAddTwoAgentsAtSamePosition() {
            ISpatialEntity a1 = GenerateAgent(1, 1);
            ISpatialEntity a2 = GenerateAgent(1, 1);
            Assert.True(_esc.Add(a1, new Vector3(0, 0)));
            Assert.False(_esc.Add(a2, new Vector3(0, 0)));
            Assert.False(_esc.Add(a2, new Vector3(0.9, 0.9)));
            Assert.True(_esc.Add(a2, new Vector3(1, 0)));
        }

        [Test]
        public void TestMoveAgentForCollision()
        {
            ISpatialEntity a1 = GenerateAgent(1, 1);
            ISpatialEntity a2 = GenerateAgent(1, 1);
            Assert.True(_esc.Add(a1, new Vector3(0, 0)));
            Assert.True(_esc.Add(a2, new Vector3(1, 1)));
            Console.WriteLine("before move " + a2.Shape.Bounds);
            Assert.False(_esc.Move(a2, new Vector3(-1, -1)).Success);
            Console.WriteLine("after move " + a2.Shape.Bounds);
        }

        [Test]
        public void TestPlaceEnvironmentAtSamePosition() {
            ISpatialEntity a1 = GenerateAgent(10, 10, CollisionType.StaticEnvironment);
            ISpatialEntity a2 = GenerateAgent(10, 10, CollisionType.StaticEnvironment);
            Assert.True(_esc.Add(a1, new Vector3(0, 0)));
            Assert.True(_esc.Add(a2, new Vector3(0, 0)));
        }

        [Test]
        public void TestAddWithRandomPositionToFillRoom() {
            for (int i = 0; i < 4; i++) {
                ISpatialEntity a = GenerateAgent(0.9d, 0.9d);
                Assert.True(_esc.AddWithRandomPosition(a, new Vector3(0, 0), new Vector3(1, 1), true));
                PrintAllAgents();
            }
            Assert.True(_esc.ExploreAll().Count() == 4);

            ISpatialEntity a2 = GenerateAgent(1, 1);
            Assert.False(_esc.AddWithRandomPosition(a2, new Vector3(0, 0), new Vector3(1, 1), true));
        }

        [Test]
        public void TestManyAgentsWithRandomPosition() {
            for (int i = 0; i < 100; i++) {
                ISpatialEntity a = GenerateAgent(1, 1);
                Assert.True(_esc.AddWithRandomPosition(a, new Vector3(0, 0), new Vector3(100, 100), false));
            }
            for (int i = 0; i < 100; i++) {
                ISpatialEntity a = GenerateAgent(1, 1);
                Assert.True(_esc.AddWithRandomPosition(a, new Vector3(0, 0), new Vector3(100, 100), false));
            }
        }

        [Test]
        public void TestCorrectPlacement2D() {
            ISpatialEntity a1 = GenerateAgent(0.9, 0.9);
            ISpatialEntity a2 = GenerateAgent(0.9, 0.9);
            ISpatialEntity a3 = GenerateAgent(0.9, 0.9);
            ISpatialEntity a4 = GenerateAgent(0.9, 0.9);

            Vector3 pos = new Vector3(1, 1, 0);
            Assert.True(_esc.Add(a1, pos));

            pos = new Vector3(2, 1, 0);
            Assert.True(_esc.Add(a2, pos));

            pos = new Vector3(2, 0, 0);
            Assert.True(_esc.Add(a3, pos));

            pos = new Vector3(0, 2, 0);
            Assert.True(_esc.Add(a4, pos));
        }

        protected void PrintAllAgents() {
            Console.WriteLine(_esc.ExploreAll().Count() + " Agents found.");
            foreach (ISpatialEntity entity in _esc.ExploreAll()) {
                Console.WriteLine(entity + " " + entity.Shape.Position);
            }
            Console.WriteLine("---");
        }

        [Test]
        public void TestPerfomanceAdd500Elements() {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 500; i++) {
                ISpatialEntity a1 = GenerateAgent(0.9, 0.9);
                Assert.True(_esc.Add(a1, new Vector3(i, 0)));
            }
            // 4.9 sec für 5k agents.
            Console.WriteLine(stopwatch.ElapsedMilliseconds + " ms");
        }

        [Test]
        public void TestPerfomanceMove50ElementsFor100Ticks() {
            int amount = 50;
            int ticks = 500;
            Stopwatch initTime = Stopwatch.StartNew();
            List<ISpatialEntity> agents = new List<ISpatialEntity>();
            for (int i = 0; i < amount; i++) {
                ISpatialEntity a1 = GenerateAgent(1, 1);
                agents.Add(a1);
                Assert.True(_esc.AddWithRandomPosition(a1, new Vector3(0, 0), new Vector3(70, 70), false));
            }
            Console.WriteLine(initTime.ElapsedMilliseconds + " ms");

            Stopwatch moveTime = Stopwatch.StartNew();
            Random random = new Random();

            int collisions = 0;
            int movementSucess = 0;
            for (int i = 0; i < ticks; i++) {
                foreach (ISpatialEntity agent in agents) {
                    if (_esc.Move(agent, new Vector3(random.Next(-2, 2), random.Next(-2, 2))).Success) {
                        movementSucess++;
                    }
                    else {
                        collisions++;
                    }
                }
            }
            Console.WriteLine("Collisions: " + collisions);
            Console.WriteLine("Movement succeeded: " + movementSucess);
            Console.WriteLine(moveTime.ElapsedMilliseconds + " ms");
        }


        ////        [Test]
        ////        public void TestIntersections() {
        ////            ESCRectAdapter adapter = new ESCRectAdapter(_esc, new Vector(1000, 1000), false);
        ////
        ////            Vector pos1 = new Vector(5d, 10.025d, 0d);
        ////            Vector pos2 = new Vector(10.026d, 7.76d, 0d);
        ////            // The agents do not collide. They touch at point (10,10). Exception is thrown if ESC thinks there is a collision.
        ////            TestSpatialAgent a1 = new TestSpatialAgent
        ////                (exec, adapter, pos1, new Vector(10d, 0.05d, 0.4d), new Direction());
        ////            TestSpatialAgent a2 = new TestSpatialAgent
        ////                (exec, adapter, pos2, new Vector(0.05d, 4.5d, 0.4d), new Direction());
        ////
        ////            Assert.True(a1.GetPosition().Equals(pos1));
        ////            Assert.True(a2.GetPosition().Equals(pos2));
        ////        }
        ////
        ////        [Test]
        ////        public void TestSpatialAgentPlacementAboveAndBelowObstacle() {
        ////            ESCRectAdapter adapter = new ESCRectAdapter(_esc, new Vector(1000, 1000), false);
        ////
        ////            // centerY = 10
        ////            Vector posObstacle = new Vector(10d, 10d, 0d);
        ////            // minY = 10 - 0.025 = 9.975, maxY = 10 + 0.025 = 10.025
        ////            Vector dimObstacle = new Vector(10d, 0.05d, 0.4d);
        ////
        ////            // centerY = 9.75
        ////            Vector posPedestrianAbove = new Vector(10d, 9.75d, 0d);
        ////            // minY = 9.75 - 0.2 = 9.55, maxY = 9.75 + 0.2 = 9.95
        ////            Vector dimPedestrianAbove = new Vector(0.4d, 0.4d, 0.4d);
        ////            // pedestrian1 is 0.025 above obstacle (not touching)
        ////
        ////            // centerY = 10.25
        ////            Vector posPedestrianBelow = new Vector(10d, 10.25d, 0d);
        ////            // minY = 10.25 - 0.2 = 10.05, maxY = 10.25 + 0.2 = 10.45
        ////            Vector dimPedestrianBelow = new Vector(0.4d, 0.4d, 0.4d);
        ////            // pedestrian1 is 0.025 below obstacle (not touching)
        ////
        ////            new TestSpatialAgent(exec, adapter, posObstacle, dimObstacle, new Direction());
        ////            PrintAllAgents();
        ////            new TestSpatialAgent(exec, adapter, posPedestrianBelow, dimPedestrianBelow, new Direction());
        ////            PrintAllAgents();
        ////            new TestSpatialAgent(exec, adapter, posPedestrianAbove, dimPedestrianAbove, new Direction());
        ////            PrintAllAgents();
        ////        }

        //        [Test]
        //        public void TestMoveTwoAgentsOnSamePosition() {
        //            ISpatialEntity a1 = GenerateAgent(10, 10);
        //            ISpatialEntity a2 = GenerateAgent(10, 10);
        //            Assert.True(_esc.Add(a1, new TVector(0, 0)));
        //            Assert.True(_esc.Add(a2, new TVector(15, 15)));
        //            Assert.False(_esc.Move(a1, new TVector(15, 15)).Success);
        //            Assert.True(_esc.Move(a2, new TVector(30, 30)).Success);
        //            Assert.True(_esc.Move(a1, new TVector(15, 15)).Success);

        //            PrintAllAgents();
        //        }

        //        protected void PrintAllAgents() {
        //            Console.WriteLine(_esc.ExploreAll().Count() + " Agents found.");
        //            foreach (ISpatialEntity entity in _esc.ExploreAll()) {
        //                RectShape rectShape = entity.Shape as RectShape;
        //                Rect bounds = rectShape.Bounds;
        //                Console.WriteLine
        //                    (entity + " " + rectShape.GetPosition() + " // " + bounds.TopLeft + " -> " + bounds.BottomRight);
        //            }
        //            Console.WriteLine("---");
        //        }

        protected TestEntity GenerateAgent(double x, double y) {
            return GenerateAgent(x, y, CollisionType.MassiveAgent);
        }

        protected TestEntity GenerateAgent(double x, double y, CollisionType collisionType) {
            return new TestEntity(x, y, collisionType);
        }
    }

}