using DalskiAgent.Environments;
using DalskiAgent.Execution;
using EnvironmentServiceComponent.Implementation;
using ESCTest.Entities;
using NUnit.Framework;
using SpatialCommon.Datatypes;
using SpatialCommon.Interfaces;

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
            return new RectAgent(x, y);
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
    }

}