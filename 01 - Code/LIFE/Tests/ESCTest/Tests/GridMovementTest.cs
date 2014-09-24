using CommonTypes.DataTypes;
using ESCTestLayer.Implementation;
using ESCTestLayer.Interface;
using GenericAgentArchitecture.Movement;
using NUnit.Framework;

namespace ESCTest.Tests {
    public class GridMovementTest {
        private const int InformationType = 1;

        #region Setup / Tear down

        [SetUp]
        public void SetUp() {
            _esc = new ESC();
            agentId1 = 1;
            agentId2 = 2;
            dimension = new Vector(1, 1);
            startPosition = Vector.Origin;
        }


        [TearDown]
        public void TearDown() {}

        #endregion

        private IESC _esc;
        private int agentId1, agentId2;
        private Vector dimension, startPosition;

        [Test]
        public void TestGridMovementSimple() {
            GridMovement m = new GridMovement(_esc, agentId2, startPosition, dimension);
            const int movementPoints = 10;
            Vector destination = new Vector(1, 3);
            m.MoveToPosition(destination, movementPoints);
            Assert.True(destination.Equals(m.Position));
        }

        [Test]
        public void TestGridMovementUnsufficentMovementPoints() {
            var m = new GridMovement(_esc, agentId2, startPosition, dimension);
            const int movementPoints = 5;
            var destination = new Vector(10, 0);
            m.MoveToPosition(destination, movementPoints);
            Assert.True(m.Position.Equals(new Vector(5, 0)));
        }

        [Test]
        public void TestGridMovementHindrance() {
            _esc.Add(agentId1, InformationType, true, dimension);
            _esc.SetPosition(agentId1, new Vector(1, 4), new Vector(1, 0));

            var m = new GridMovement(_esc, agentId2, startPosition, dimension);
            const int movementPoints = 10;

            var firstDestination = new Vector(1, 3);
            m.MoveToPosition(firstDestination, movementPoints);
            Assert.True(firstDestination.Equals(m.Position));

            var secondDestination = new Vector(1, 4);
            m.MoveToPosition(secondDestination, movementPoints);

            Assert.True(firstDestination.Equals(m.Position));
            Assert.False(secondDestination.Equals(m.Position));
        }
    }
}