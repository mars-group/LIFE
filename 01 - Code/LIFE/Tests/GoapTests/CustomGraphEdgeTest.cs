using System;
using System.Collections.Generic;
using GoapCommon.Implementation;
using GoapModelTest.Actions;
using GoapModelTest.Worldstates;
using NUnit.Framework;

namespace GoapTests {

    [TestFixture]
    internal class CustomGraphEdgeTest {
        private static readonly Node V1 = new Node
            (new List<WorldstateSymbol> {new WorldstateSymbol(WorldProperties.Happy, true, typeof (Boolean))},
                new List<WorldstateSymbol>(),
                1);

        private static readonly Node V2 = new Node
            (new List<WorldstateSymbol> {new WorldstateSymbol(WorldProperties.Happy, true, typeof (Boolean))},
                new List<WorldstateSymbol> {new WorldstateSymbol(WorldProperties.HasToy, false, typeof (Boolean))},
                1);

        private static readonly Node V3 = new Node
            (new List<WorldstateSymbol> {new WorldstateSymbol(WorldProperties.HasToy, false, typeof (Boolean))},
                new List<WorldstateSymbol>(),
                1);

        private static readonly Edge E1 = new Edge(new ActionClean(), V1, V2);
        private static readonly Edge E2 = new Edge(new ActionClean(), V1, V2);
        private static readonly Edge E3 = new Edge(new ActionGetToy(), V2, V3);

        [Test]
        public void SourceVertexTest() {
            Assert.AreEqual(E1.GetSource(), V1);
            Assert.AreEqual(E2.GetSource(), V1);
            Assert.AreEqual(E3.GetSource(), V2);
        }

        [Test]
        public void TargetVertexTest() {
            Assert.AreEqual(E1.GetTarget(), V2);
            Assert.AreEqual(E3.GetTarget(), V3);
            Assert.AreEqual(E3.GetTarget(), V3);
        }

        [Test]
        public void EqualityTest() {
            Assert.AreEqual(E1, E1);
            Assert.AreEqual(E1, E2);
        }

        [Test]
        public void NoEqualityTest() {
            Assert.AreNotEqual(E2, E3);
            Assert.AreNotEqual(E2, null);
        }

        [Test]
        public void EqualityOperatorTest() {
            Assert.True(E1 != null);
            Assert.False(E1 == E3);
            Assert.False(E2 == null);
        }
    }

}