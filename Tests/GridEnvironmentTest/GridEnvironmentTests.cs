using System;
using System.Linq;
using System.Threading.Tasks;
using LIFE.API.GridCommon;
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

            // explore 'em
            Assert.DoesNotThrow(() =>
            {
                var res = _env.Explore(GetRandomCoord(), maxNumberOfResults: 1);
                Assert.IsNotEmpty(res);
            });

            // explore 'em all
            var res2 = _env.Explore(GetRandomCoord());
            Assert.IsTrue(res2.Count() == agentCount);
        }

        [Test]
        public void TestExploreSpecific()
        {
            var agent = new Tree(new GridCoordinate(42,23));
            _env.Insert(agent);
            var res = _env.Explore(agent.Coord, 0);
            Assert.IsTrue(res.Count() == 1);
            Assert.AreEqual(res.First(), agent);
        }

        private GridCoordinate GetRandomCoord()
        {
            return new GridCoordinate(_random.Next(DimensionX), _random.Next(DimensionY));
        }
    }



    internal class Tree : IEquatable<Tree>, IGridCoordinate {
        public Tree(GridCoordinate cord) {
            TreeId = Guid.NewGuid();
            Coord = cord;
        }

        public GridCoordinate Coord { get; }

        public Guid TreeId { get; }



        public int X => Coord.X;
        public int Y => Coord.Y;
        public GridDirection GridDirection { get; }
        public bool Equals(Tree other)
        {
            return this.TreeId.Equals(other.TreeId);
        }

        public bool Equals(IGridCoordinate other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }
    }
}