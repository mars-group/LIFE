using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EnvironmentServiceComponent.Entities;
using ESCTest.Entities;
using NUnit.Framework;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Movement;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Environment;
using SpatialAPI.Shape;

namespace ESCTest.Tests {

    public class TestESC {
        [Test] // Do a number of insertion tests to measure general performance.
        public void PerformanceTest() {
            // Test series.
            int[] tests = {1000, 2000, 4000, 8000, 16000, 32000, 64000, 128000};
            const int envWidth = 1000; // Important: Ensure that width*height
            const int envHeight = 1000; // is sufficient to store max. number of agents!

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Perform tests and output elapsed time.
            for (int t = 0; t < tests.Length; t++) {
                IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
                for (int i = 0; i < tests[t]; i++) {
                    var b1 = BoundingBox.GenerateByDimension
                        (Vector3.Zero, new Vector3(0.9, 0.9, 0.9));
                    var t1 = new TestEntity(b1);
                    Assert.IsTrue(esc.Add(t1, new Vector3(i%envWidth, i/envWidth)));
                }
                Console.WriteLine("[" + tests[t] + " agents]: " + stopwatch.ElapsedMilliseconds + " ms");
                stopwatch.Restart(); // Reset stopwatch.
            }
        }

//        [Test]
//        public void TestSkukuza() {
//            var dim = 0.0000000000000001;
//            IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
//            int counter = 0;
//            string line;
//
//            StreamReader file = new StreamReader(@"C:\Users\Olfi\Desktop\Skukuza_Trees_LatLon.txt");
//
//            while ((line = file.ReadLine()) != null) {
//                Console.WriteLine(line);
//                var split = line.Split(';');
//                try {
//                    var x = Convert.ToDouble(split[0]);
//                    var y = Convert.ToDouble(split[1]);
//                    ISpatialEntity a1 = GenerateAgent(dim, dim);
//                    Assert.True(esc.Add(a1, new Vector3(x, y)));
//                }
//                catch (System.FormatException e) {
//                    //no usable values in this row
//                }
//                counter++;
//            }
//            Console.WriteLine(esc.ExploreAll().Count());
//            file.Close();
//        }

        [Test]
        public void TestExploreEntity() {
            IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            ISpatialEntity a1 = GenerateAgent(2, 2);
            ISpatialEntity a2 = GenerateAgent(1, 1);
            Assert.True(esc.Add(a1, Vector3.Zero));
            Assert.True(esc.Add(a2, new Vector3(10, 10)));
            Assert.True(esc.Explore(new ExploreEntity(a1)*10).Count() == 2);
        }

        [Test]
        public void TestAddFirstAgent() {
            IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            ISpatialEntity a1 = GenerateAgent(1, 1);
            Assert.True(esc.Add(a1, Vector3.Zero));
        }

        [Test]
        public void TestAddTwoAgentsAtAdjacentPositions() {
            IEnvironment esc;
            esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            ISpatialEntity a1 = GenerateAgent(1, 1);
            ISpatialEntity a2 = GenerateAgent(1, 1);
            Assert.True(esc.Add(a1, new Vector3(0, 0)));
            Assert.False(esc.Add(a2, new Vector3(0.99, 0)));
            Assert.True(esc.Add(a2, new Vector3(1, 0)));
        }

        [Test]
        public void TestAddTwoAgentsAtSamePosition() {
            IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            ISpatialEntity a1 = GenerateAgent(1, 1);
            ISpatialEntity a2 = GenerateAgent(1, 1);
            Assert.True(esc.Add(a1, new Vector3(0, 0)));
            Assert.False(esc.Add(a2, new Vector3(0, 0)));
            Assert.False(esc.Add(a2, new Vector3(0.9, 0.9)));
            Assert.True(esc.Add(a2, new Vector3(1, 0)));
        }

        [Test]
        public void TestMoveAgentForCollision() {
            IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            ISpatialEntity a1 = GenerateAgent(1, 1);
            ISpatialEntity a2 = GenerateAgent(1, 1);
            Assert.True(esc.Add(a1, new Vector3(0, 0)));
            Assert.True(esc.Add(a2, new Vector3(1, 1)));
            Console.WriteLine("before move " + a2.Shape.Bounds);
            Assert.False(esc.Move(a2, new Vector3(-1, -1)).Success);
            Console.WriteLine("after move " + a2.Shape.Bounds);
        }

        [Test]
        public void TestPlaceEnvironmentAtSamePosition() {
            IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            ISpatialEntity a1 = GenerateAgent(10, 10, CollisionType.StaticEnvironment);
            ISpatialEntity a2 = GenerateAgent(10, 10, CollisionType.StaticEnvironment);
            Assert.True(esc.Add(a1, new Vector3(0, 0)));
            Assert.True(esc.Add(a2, new Vector3(0, 0)));
        }

        [Test]
        public void TestAddWithRandomPositionToFillRoom() {
            IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            for (int i = 0; i < 4; i++) {
                ISpatialEntity a = GenerateAgent(1, 1);
                Assert.True(esc.AddWithRandomPosition(a, new Vector3(0, 0), new Vector3(1, 1), true));
            }
            Assert.True(esc.ExploreAll().Count() == 4);

            ISpatialEntity a2 = GenerateAgent(1, 1);
            Assert.False(esc.AddWithRandomPosition(a2, new Vector3(0, 0), new Vector3(1, 1), true));
        }

        [Test]
        public void TestManyAgentsWithRandomPosition() {
            IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            for (int i = 0; i < 100; i++) {
                ISpatialEntity a = GenerateAgent(1, 1);
                Assert.True(esc.AddWithRandomPosition(a, new Vector3(0, 0), new Vector3(100, 100), false));
            }
            for (int i = 0; i < 100; i++) {
                ISpatialEntity a = GenerateAgent(1, 1);
                Assert.True(esc.AddWithRandomPosition(a, new Vector3(0, 0), new Vector3(100, 100), false));
            }
        }

        [Test]
        public void TestCorrectPlacement2D() {
            IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            ISpatialEntity a1 = GenerateAgent(0.9, 0.9);
            ISpatialEntity a2 = GenerateAgent(0.9, 0.9);
            ISpatialEntity a3 = GenerateAgent(0.9, 0.9);
            ISpatialEntity a4 = GenerateAgent(0.9, 0.9);

            Vector3 pos = new Vector3(1, 1, 0);
            Assert.True(esc.Add(a1, pos));

            pos = new Vector3(2, 1, 0);
            Assert.True(esc.Add(a2, pos));

            pos = new Vector3(2, 0, 0);
            Assert.True(esc.Add(a3, pos));

            pos = new Vector3(0, 2, 0);
            Assert.True(esc.Add(a4, pos));
        }

//        protected void PrintAllAgents() {
//            Console.WriteLine(esc.ExploreAll().Count() + " Agents found.");
//            foreach (ISpatialEntity entity in esc.ExploreAll()) {
//                Console.WriteLine(entity + " " + entity.Shape.Position);
//            }
//            Console.WriteLine("---");
//        }

        [Test]
        public void TestPerfomanceAdd500Elements() {
            IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 500; i++) {
                ISpatialEntity a1 = GenerateAgent(0.9, 0.9);
                Assert.True(esc.Add(a1, new Vector3(i, 0)));
            }
            // 4.9 sec für 5k agents.
            Console.WriteLine(stopwatch.ElapsedMilliseconds + " ms");
        }

        [Test]
        public void TestPerfomanceMove50ElementsFor100Ticks() {
            IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            int amount = 3;
            int ticks = 500;
            Stopwatch initTime = Stopwatch.StartNew();
            List<ISpatialEntity> agents = new List<ISpatialEntity>();
            for (int i = 0; i < amount; i++) {
                ISpatialEntity a1 = GenerateAgent(1, 1);
                agents.Add(a1);
                Assert.True(esc.AddWithRandomPosition(a1, new Vector3(0, 0), new Vector3(70, 70), false));
            }
            Console.WriteLine(initTime.ElapsedMilliseconds + " ms");

            Stopwatch moveTime = Stopwatch.StartNew();
            Random random = new Random();

            int collisions = 0;
            int movementSucess = 0;
            for (int i = 0; i < ticks; i++) {
                foreach (ISpatialEntity agent in agents) {
                    if (esc.Move(agent, new Vector3(random.Next(-2, 2), random.Next(-2, 2))).Success) {
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

//        [Test]
//        public void TestIntersections() {
//            IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
//
//            Vector3 pos1 = new Vector3(5d, 10.025d, 0d);
//            Vector3 pos2 = new Vector3(10.026d, 7.76d, 0d);
//            // The agents do not collide. They touch at point (10,10). Exception is thrown if ESC thinks there is a collision.
//            TestEntity a1 = new TestEntity
//                (pos1, new Vector3(10d, 0.05d, 0.4d), new Direction());
//            TestEntity a2 = new TestEntity
//                (pos2, new Vector3(0.05d, 4.5d, 0.4d), new Direction());
//
//            Assert.True(a1.Position.Equals(pos1));
//            Assert.True(a2.Position.Equals(pos2));
//        }

//                [Test]
//                public void TestSpatialAgentPlacementAboveAndBelowObstacle()
//                {
//                    IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
//                    ESCRectAdapter adapter = new ESCRectAdapter(esc, new Vector3(1000, 1000), false);
//        
//                    // centerY = 10
//                    Vector3 posObstacle = new Vector3(10d, 10d, 0d);
//                    // minY = 10 - 0.025 = 9.975, maxY = 10 + 0.025 = 10.025
//                    Vector3 dimObstacle = new Vector3(10d, 0.05d, 0.4d);
//        
//                    // centerY = 9.75
//                    Vector3 posPedestrianAbove = new Vector3(10d, 9.75d, 0d);
//                    // minY = 9.75 - 0.2 = 9.55, maxY = 9.75 + 0.2 = 9.95
//                    Vector3 dimPedestrianAbove = new Vector3(0.4d, 0.4d, 0.4d);
//                    // pedestrian1 is 0.025 above obstacle (not touching)
//        
//                    // centerY = 10.25
//                    Vector3 posPedestrianBelow = new Vector3(10d, 10.25d, 0d);
//                    // minY = 10.25 - 0.2 = 10.05, maxY = 10.25 + 0.2 = 10.45
//                    Vector3 dimPedestrianBelow = new Vector3(0.4d, 0.4d, 0.4d);
//                    // pedestrian1 is 0.025 below obstacle (not touching)
//        
//                    new TestSpatialAgent(exec, adapter, posObstacle, dimObstacle, new Direction());
//                    new TestSpatialAgent(exec, adapter, posPedestrianBelow, dimPedestrianBelow, new Direction());
//                    new TestSpatialAgent(exec, adapter, posPedestrianAbove, dimPedestrianAbove, new Direction());
//                }

        [Test]
        public void TestMoveTwoAgentsOnSamePosition() {
            IEnvironment esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            ISpatialEntity a1 = GenerateAgent(10, 10);
            ISpatialEntity a2 = GenerateAgent(10, 10);
            Assert.True(esc.Add(a1, new Vector3(0, 0)));
            Assert.True(esc.Add(a2, new Vector3(15, 15)));
            Assert.False(esc.Move(a1, new Vector3(15, 15)).Success);
            Assert.True(esc.Move(a2, new Vector3(30, 30)).Success);
            Assert.True(esc.Move(a1, new Vector3(15, 15)).Success);
        }

        protected TestEntity GenerateAgent(double x, double y) {
            return GenerateAgent(x, y, CollisionType.MassiveAgent);
        }

        protected TestEntity GenerateAgent(double x, double y, CollisionType collisionType) {
            return new TestEntity(x, y, collisionType);
        }
    }

}