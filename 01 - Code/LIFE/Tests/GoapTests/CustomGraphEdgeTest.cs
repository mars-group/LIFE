using System.Collections.Generic;
using GoapCommon.Interfaces;
using GoapGraphConnector.CustomGraph;
using NUnit.Framework;

namespace GoapTests {
    [TestFixture]
    internal class CustomGraphEdgeTest {

        private static readonly Vertex V1 = new Vertex(new List<IGoapWorldProperty>(), 2, "V1");
        private static readonly Vertex V2 = new Vertex(new List<IGoapWorldProperty>(), 2, "V2");
        private static readonly Vertex V3 = new Vertex(new List<IGoapWorldProperty>(), 2, "V3");
        private static readonly Vertex V4 = new Vertex(new List<IGoapWorldProperty>(), 2, "V4");

        private static readonly Edge E1 = new Edge(1, V1, V2);
        private static readonly Edge E2 = new Edge(2, V2, V3);
        private static readonly Edge E3 = new Edge(3, V2, V4);
        
        [Test]
        public void SourceVertexTest() {
            Assert.AreEqual(E1.GetSource(),V1);
            Assert.True(E1.GetSource().GetIdentifier() == "V1");
        }

        [Test]
        public void TargetVertexTest() {
            Assert.AreEqual(E1.GetTarget(),V2);
            Assert.True(E1.GetTarget().GetIdentifier() == "V2");
        }

        [Test]
        public void EqualityTest() {
            Assert.AreEqual(E1,E1);
        }

        [Test]
        public void NoEqualityTest() {
            Assert.AreNotEqual(E1,E2);
            Assert.AreNotEqual(E2,E3);
            Assert.AreNotEqual(E2,null);
        }

        [Test]
        public void EqualityOperatorTest() {
            Assert.True(E1 != null);
            Assert.False(E1 == E3);
            Assert.False(E2 == null);
        }
    }
}