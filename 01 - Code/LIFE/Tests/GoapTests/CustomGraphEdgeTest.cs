using System.Collections.Generic;
using GoapCommon.Interfaces;
using GoapGraphConnector.CustomGraph;
using NUnit.Framework;

namespace GoapTests {
    [TestFixture]
    internal class CustomGraphEdgeTest {
        #region Setup/Teardown

        [SetUp]
        protected void SetUp() {
            _v1 = new Vertex(new List<IGoapWorldstate>(), 2, "V1");
            _v2 = new Vertex(new List<IGoapWorldstate>(), 2, "V2");
            _v3 = new Vertex(new List<IGoapWorldstate>(), 2, "V3");
            _v4 = new Vertex(new List<IGoapWorldstate>(), 2, "V4");


            _e1 = new Edge(1, _v1, _v2);
            _e2 = new Edge(2, _v2, _v3);
            _e3 = new Edge(3, _v2, _v4);
        }

        #endregion

        private Edge _e1;
        private Edge _e2;
        private Edge _e3;

        private Vertex _v1;
        private Vertex _v2;
        private Vertex _v3;
        private Vertex _v4;

        [Test]
        public void SourceVertexTest() {
            Assert.True(_e1.GetSource().Equals(_v1));
            Assert.True(_e1.GetSource().GetIdentifier() == "V1");
        }

        [Test]
        public void TargetVertexTest() {
            Assert.True(_e1.GetTarget().Equals(_v2));
            Assert.True(_e1.GetTarget().GetIdentifier() == "V2");
        }

        [Test]
        public void EqualityTest() {
            Assert.True(_e1.Equals(_e1));
            Assert.False(_e1.Equals(_e2));
            Assert.False(_e1.Equals(null));
        }

        [Test]
        public void EqualityOperatorTest() {
            Assert.True(_e1 != null);
            Assert.False(_e1 == _e3);
            Assert.False(_e2 == null);
        }
    }
}