namespace ESCTest.Tests {
    using System;
    using System.Diagnostics;
    using System.Linq;
    using CommonTypes.TransportTypes;
    using Entities;
    using ESCTestLayer.Implementation;
    using ESCTestLayer.Interface;
    using NetTopologySuite.Geometries;
    using NUnit.Framework;

    public class UnboundESCTest {
        private const int InformationType = 1;

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
        public void TestAddTwoAgentsAtSamePosition()
        {
            TestAgent2D a1 = new TestAgent2D(1, 1);
            TestAgent2D a2 = new TestAgent2D(1, 1);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.False(_esc.Add(a2, new TVector(0, 0)));
            Assert.False(_esc.Add(a2, new TVector(0.5f, 0.5f)));
            Assert.True(_esc.Add(a2, new TVector(1, 1)));
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
        public void TestCorrectPlacement2D()
        {
            TestAgent2D a1 = new TestAgent2D(1, 1);
            TestAgent2D a2 = new TestAgent2D(1, 1);
            TestAgent2D a3 = new TestAgent2D(1, 1);
            TestAgent2D a4 = new TestAgent2D(1, 1);

            var pos = new TVector(1, 1, 0);
            Assert.True(_esc.Add(a1, pos));

            pos = new TVector(2, 1, 0);
            Assert.True(_esc.Add(a2, pos));
//            Assert.True(pos.Equals(_esc.Move(a2, pos, new TVector(-1, 0, 0)).Success));

            pos = new TVector(2, 0, 0);
            Assert.True(_esc.Add(a3, pos));
//            Assert.True(pos.Equals(_esc.Move(a3, pos, new TVector(0, -1, 0)).Success));

            pos = new TVector(0, 2, 0);
            Assert.True(_esc.Add(a4, pos));
//            Assert.True(pos.Equals(_esc.Move(a4, pos, new TVector(1, 0, 0)).Success));
        }


        private void printAllAgents() {
            Console.WriteLine(_esc.ExploreAll().Count()+ " Agents found.");
            foreach (var entity in _esc.ExploreAll()) {
                Console.WriteLine(entity.Geometry);
            }
            Console.WriteLine("---");
        }



        [Test]
        public void TestRegainOfOldPosition() {
            TestAgent2D a1 = new TestAgent2D(1, 1);
            TestAgent2D a2 = new TestAgent2D(1, 1);

            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            Assert.True(_esc.Add(a2, new TVector(0, 1)));

            var oldCentroid = a2.Geometry.Centroid;
            var movementResult = _esc.Move(a2, new TVector(0, -1));
            Assert.False(movementResult.Success);
            Assert.True(movementResult.Collisions.Contains(a1));
            Assert.True(a2.Geometry.Centroid.Equals(oldCentroid));
        }

        [Test]
        public void TestGeometryFactory() {
//            new MyTestGeometryFactory();
            Console.WriteLine(MyGeometryFactory.Rectangle(10, 4));
        }

        [Test]
        public void TestRotation() {
            TestAgent2D a1 = new TestAgent2D(10, 1);
            Assert.True(_esc.Add(a1, new TVector(0, 0)));
            printAllAgents();
            Assert.True(_esc.Move(a1, new TVector(0, 0), 360).Success);
            printAllAgents();
        }

        [Test]
        public void TestAdd500Elements() {
            var dimension = new TVector(1, 1, 1);

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 500; i++)
            {
                TestAgent2D a1 = new TestAgent2D(1, 1);
                Assert.True(_esc.Add(a1, new TVector(i, 0)));
            }
            // 36.4 sec für 50k agents.
            Console.WriteLine(stopwatch.ElapsedMilliseconds+ " ms");
        }
    }
}