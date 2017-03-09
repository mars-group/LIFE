using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using LIFE.API.Environment.GridCommon;
using LIFE.Components.Environments.GridEnvironment;
using NUnit.Framework;

namespace GridEnvironmentTest
{
    [TestFixture]
    public class Tests
    {
        private IGridEnvironment<Tree> _env;
        private readonly Random _random = new Random();
        private const int DimensionX = 60;
        private const int DimensionY = 40;

        [SetUp]
        public void Setup()
        {
            _env = new GridEnvironment<Tree>(DimensionX, DimensionY);
        }

        [Test]
        public void TestInsert()
        {
            var agentCount = 10000;

            for (var i = 0; i < agentCount; i++)
            {

                Assert.DoesNotThrow(() =>
                {
                    var agent = new Tree(GetRandomCoord());
                    _env.Insert(agent);
                });
            }
        }

        [Test]
        public void TestInsertParallel()
        {
            var agentCount = 10000;

            Parallel.For(0, agentCount, i =>
            {
                Assert.DoesNotThrow(() =>
                {
                    var agent = new Tree(GetRandomCoord());
                    _env.Insert(agent);
                });
            });
        }

        [Test]
        public void TestMoveToPosition()
        {
            // insert 10K agents
            var agentCount = 10000;
            var agents = new ConcurrentBag<Tree>();
            Parallel.For(0, agentCount, i =>
            {
                Assert.DoesNotThrow(() =>
                {
                    var agent = new Tree(GetRandomCoord());
                    agents.Add(agent);
                    _env.Insert(agent);
                });
            });
            var a = agents.First();

            // normale move to new position
            var target = new GridCoordinate(50,32);
            var newPos = _env.MoveToPosition(a, target);
            a.Coord = newPos;
            Assert.AreEqual(target, newPos);
            Assert.AreEqual(a.Coord, newPos);

            // move outside of bounds
            var target2 = new GridCoordinate(60,41);
            var newPos2 = _env.MoveToPosition(a, target2);
            Assert.AreEqual(a.Coord, newPos2);
            Assert.AreNotEqual(newPos2, target2);
        }

        [Test]
        public void TestMoveTowardsTarget()
        {
            // insert 10K agents
            var agentCount = 10000;
            var agents = new ConcurrentBag<Tree>();
            Parallel.For(0, agentCount, i =>
            {
                Assert.DoesNotThrow(() =>
                {
                    var agent = new Tree(GetRandomCoord());
                    agents.Add(agent);
                    var success = _env.Insert(agent);
                    Assert.IsTrue(success);
                });
            });
            var a = agents.First();

            IGridCoordinate target, oldPositionOfA;

            // distance=0 should do a 'move' to my position
            a.Coord = _env.MoveToPosition(a, 0, 0);
            Assert.AreEqual(new GridCoordinate(0,0), a.Coord);
            target = new GridCoordinate(a.X + 10, a.Y);
            oldPositionOfA = a.Coord;
            a.Coord = _env.MoveTowardsTarget(a, target, 0);
            Assert.AreEqual(oldPositionOfA, a.Coord);


            // normal move to new position with distance = 1
            a.Coord = _env.MoveToPosition(a, 0, 0);
            Assert.AreEqual(new GridCoordinate(0,0), a.Coord);
            target = new GridCoordinate(a.X + 10, a.Y);
            oldPositionOfA = a.Coord;
            a.Coord = _env.MoveTowardsTarget(a, target, 1);
            Assert.AreEqual(oldPositionOfA.X + 1, a.Coord.X);

            // exact move to target
            a.Coord = _env.MoveToPosition(a, 0, 0);
            Assert.AreEqual(new GridCoordinate(0,0), a.Coord);
            target = new GridCoordinate(a.X + 10, a.Y);
            a.Coord = _env.MoveTowardsTarget(a, target, 10);
            Assert.AreEqual(target, a.Coord);

            // high-speed move to target, should not go further than target
            a.Coord = _env.MoveToPosition(a, 0, 0);
            Assert.AreEqual(new GridCoordinate(0,0), a.Coord);
            target = new GridCoordinate(a.X + 10, a.Y);
            a.Coord = _env.MoveTowardsTarget(a, target, 100);
            Assert.AreEqual(target, a.Coord);

            // distance=0 should do a 'move' to my position
            a.Coord = _env.MoveToPosition(a, 0, 0);
            Assert.AreEqual(new GridCoordinate(0,0), a.Coord);
            target = new GridCoordinate(a.X + 10, a.Y);
            oldPositionOfA = a.Coord;
            a.Coord = _env.MoveTowardsTarget(a, target, -100);
            Assert.AreEqual(oldPositionOfA, a.Coord);
        }

        [Test]
        public void TestExplore()
        {
            // insert 10K agents
            var agentCount = 10000;
            Parallel.For(0, agentCount, i =>
            {
                Assert.DoesNotThrow(() =>
                {
                    var agent = new Tree(GetRandomCoord());

                    _env.Insert(agent);
                });
            });

            // explore 1
            Assert.DoesNotThrow(() =>
            {
                var res = _env.Explore(GetRandomCoord(), maxNumberOfResults: 1);
                Assert.IsNotEmpty(res);
            });

            // explore 'em all
            var res2 = _env.Explore(GetRandomCoord());
            Assert.AreEqual(agentCount, res2.Count());

            // explore 5
            var res3 = _env.Explore(GetRandomCoord(), maxNumberOfResults: 5);
            Assert.AreEqual(5, res3.Count());

            // explore out of grid
            var res4 = _env.Explore(new GridCoordinate(DimensionX+1,DimensionY+1), maxNumberOfResults: 5);
            Assert.AreEqual(0, res4.Count());
        }

        [Test]
        public void TestExploreSpecific()
        {
            var agent = new Tree(new GridCoordinate(42,23));
            _env.Insert(agent);
            var res = _env.Explore(agent, 0);
            Assert.IsTrue(res.Count() == 1);
            Assert.AreEqual(res.First(), agent);
        }


        private GridCoordinate GetRandomCoord()
        {
            return new GridCoordinate(_random.Next(DimensionX), _random.Next(DimensionY));
        }
    }

    internal class Tree : IGridCoordinate {
        public Tree(IGridCoordinate cord) {
            Coord = cord;
        }

        public IGridCoordinate Coord { get; set; }

        public int X => Coord.X;
        public int Y => Coord.Y;
        public GridDirection GridDirection { get; }

        public bool Equals(IGridCoordinate other)
        {
            return X == other.X && Y == other.Y;
        }
    }
}