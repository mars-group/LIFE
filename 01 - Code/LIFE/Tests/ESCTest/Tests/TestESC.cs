using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EnvironmentServiceComponent.Interface;
using NUnit.Framework;
using SpatialCommon.Enums;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;

namespace ESCTest.Tests {

    public abstract class TestESC {
        protected IEnvironmentServiceComponent _esc;


        [Test]
        public void TestAddFirstAgent() {
            ISpatialEntity a1 = GenerateAgent(1, 1);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
        }

        [Test]
        public void TestAddTwoAgentsAtAdjacentPositions() {
            ISpatialEntity a1 = GenerateAgent(1, 1);
            ISpatialEntity a2 = GenerateAgent(1, 1);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.False(_esc.Add(a2, new TVector(1, 0)));
            Assert.True(_esc.Add(a2, new TVector(1.1, 0)));
        }

        [Test]
        public void TestAddTwoAgentsAtSamePosition() {
            ISpatialEntity a1 = GenerateAgent(1, 1);
            ISpatialEntity a2 = GenerateAgent(1, 1);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.False(_esc.Add(a2, new TVector(0, 0)));
            Assert.False(_esc.Add(a2, new TVector(1, 1)));
            Assert.True(_esc.Add(a2, new TVector(1.1, 0)));
        }


        [Test]
        public void TestMoveAgentForCollision() {
            ISpatialEntity a1 = GenerateAgent(0.9, 0.9);
            ISpatialEntity a2 = GenerateAgent(0.9, 0.9);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.True(_esc.Add(a2, new TVector(1, 1)));
            Assert.False(_esc.Move(a2, new TVector(-1, -1)).Success);
        }

        [Test]
        public void TestPlaceEnvironmentAtSamePosition() {
            ISpatialEntity a1 = GenerateAgent(10, 10, CollisionType.StaticEnvironment);
            ISpatialEntity a2 = GenerateAgent(10, 10, CollisionType.StaticEnvironment);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.True(_esc.Add(a2, new TVector(0, 0)));
        }


        [Test]
        public void TestAddWithRandomPositionToFillRoom() {
            for (int i = 0; i < 4; i++) {
                ISpatialEntity a = GenerateAgent(0.9d, 0.9d);
                Assert.True(_esc.AddWithRandomPosition(a, new TVector(0, 0), new TVector(1, 1), true));
                PrintAllAgents();
            }
            Assert.True(_esc.ExploreAll().Count() == 4);

            ISpatialEntity a2 = GenerateAgent(1, 1);
            Assert.False(_esc.AddWithRandomPosition(a2, new TVector(0, 0), new TVector(1, 1), true));
        }


        [Test]
        public void TestManyAgentsWithRandomPosition() {
            for (int i = 0; i < 100; i++) {
                ISpatialEntity a = GenerateAgent(1, 1);
                Assert.True(_esc.AddWithRandomPosition(a, new TVector(0, 0), new TVector(100, 100), false));
            }
            for (int i = 0; i < 100; i++) {
                ISpatialEntity a = GenerateAgent(1, 1);
                Assert.True(_esc.AddWithRandomPosition(a, new TVector(0, 0), new TVector(100, 100), false));
            }
        }


        [Test]
        public void TestCorrectPlacement2D() {
            ISpatialEntity a1 = GenerateAgent(0.9, 0.9);
            ISpatialEntity a2 = GenerateAgent(0.9, 0.9);
            ISpatialEntity a3 = GenerateAgent(0.9, 0.9);
            ISpatialEntity a4 = GenerateAgent(0.9, 0.9);

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
                Console.WriteLine(entity + " " + entity.Shape.GetPosition());
            }
            Console.WriteLine("---");
        }


        [Test]
        public void TestPerfomanceAdd500Elements() {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 500; i++) {
                ISpatialEntity a1 = GenerateAgent(0.9, 0.9);
                Assert.True(_esc.Add(a1, new TVector(i, 0)));
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
                Assert.True(_esc.AddWithRandomPosition(a1, new TVector(0, 0), new TVector(70, 70), false));
            }
            Console.WriteLine(initTime.ElapsedMilliseconds + " ms");

            Stopwatch moveTime = Stopwatch.StartNew();
            Random random = new Random();

            int collisions = 0;
            int movementSucess = 0;
            for (int i = 0; i < ticks; i++) {
                foreach (ISpatialEntity agent in agents) {
                    if (_esc.Move(agent, new TVector(random.Next(-2, 2), random.Next(-2, 2))).Success) {
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

        protected abstract ISpatialEntity GenerateAgent(double x, double y);
        protected abstract ISpatialEntity GenerateAgent(double x, double y, CollisionType collisionType);
    }

}