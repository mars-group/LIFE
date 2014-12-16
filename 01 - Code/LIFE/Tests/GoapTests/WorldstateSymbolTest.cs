using System;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;
using NUnit.Framework;

namespace GoapTests {

    [TestFixture]
    internal class WorldstateSymbolTest {
        private static readonly WorldstateSymbol Ws1 = new WorldstateSymbol(WorldProperties.A, 1, typeof (int));
        private static readonly WorldstateSymbol Ws2 = new WorldstateSymbol(WorldProperties.A, 1, typeof (int));
        private static readonly WorldstateSymbol Ws3 = new WorldstateSymbol(WorldProperties.A, 2, typeof (int));

        private static readonly WorldstateSymbol Ws4 = new WorldstateSymbol(WorldProperties.A, 1, typeof (float));
        private static readonly WorldstateSymbol Ws5 = new WorldstateSymbol(WorldProperties.A, 1, typeof (float));

        private static readonly WorldstateSymbol Ws6 = new WorldstateSymbol(WorldProperties.A, true, typeof (bool));
        private static readonly WorldstateSymbol Ws7 = new WorldstateSymbol(WorldProperties.A, true, typeof (bool));
        private static readonly WorldstateSymbol Ws8 = new WorldstateSymbol(WorldProperties.A, true, typeof (Boolean));


        private static readonly WorldstateSymbol Ws9 = new WorldstateSymbol(WorldProperties.A, 2, typeof (double));


        [Test]
        public void EqualityTest() {
            Assert.True(Ws1.Equals(Ws2));
            Assert.True(Ws4.Equals(Ws5));
            Assert.True(Ws6.Equals(Ws7));
            Assert.True(Ws7.Equals(Ws8));
        }

        [Test]
        public void NotEqualityTest() {
            Assert.False(Ws1.Equals(Ws4));
            Assert.False(Ws1.Equals(Ws3));
            Assert.False(Ws3.Equals(Ws9));
        }
    }

}