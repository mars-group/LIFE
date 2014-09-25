using TVector = CommonTypes.DataTypes.Vector;
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
            _agentId1 = 1;
            _agentId2 = 2;
            _dimension = new TVector(1, 1);
            _startPosition = TVector.Origin;
        }


        [TearDown]
        public void TearDown() {}

        #endregion

        private IESC _esc;
        private int _agentId1, _agentId2;
        private TVector _dimension, _startPosition;

        [Test]
        public void TestGridMovementSimple() {
            GridMovement m = new GridMovement(_esc, new ESCInitData{ AgentID = _agentId2}, _startPosition, _dimension);
            const int movementPoints = 10;
            var destination = new TVector(1, 3);
            m.MoveToPosition(destination, movementPoints);
            Assert.True(m.Position.Equals(destination));
        }

        [Test]
        public void TestGridMovementUnsufficentMovementPoints() {
            var m = new GridMovement(_esc, new ESCInitData{ AgentID = _agentId2}, _startPosition, _dimension);
            const int movementPoints = 5;
            var destination = new TVector(10, 0);
            m.MoveToPosition(destination, movementPoints);
            Assert.True(m.Position.Equals(new Vector(5, 0)));
        }

        [Test]
        public void TestGridMovementHindrance() {
            _esc.Add(_agentId1, InformationType, true, _dimension);
            _esc.SetPosition(_agentId1, new TVector(1, 4), new TVector(1, 0));

            var m = new GridMovement(_esc, new ESCInitData{ AgentID = _agentId2}, _startPosition, _dimension);
            const int movementPoints = 10;

            var firstDestination = new TVector(1, 3);
            m.MoveToPosition(firstDestination, movementPoints);
            Assert.True(m.Position.Equals(firstDestination));

            var secondDestination = new TVector(1, 4);
            m.MoveToPosition(secondDestination, movementPoints);

            Assert.True(m.Position.Equals(firstDestination));
            Assert.False(m.Position.Equals(secondDestination));
        }
    }
}