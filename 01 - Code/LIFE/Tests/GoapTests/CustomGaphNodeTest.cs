using System;
using System.Collections.Generic;
using GoapBetaCommon.Implementation;
using GoapModelTest.Worldstates;
using NUnit.Framework;

namespace GoapTests {

    [TestFixture]
    public class CustomGaphNodeTest {
        private static readonly WorldstateSymbol HappyTrue = new WorldstateSymbol
            (WorldProperties.Happy, true, typeof (Boolean));

        private static readonly WorldstateSymbol HappyFalse1 = new WorldstateSymbol
            (WorldProperties.Happy, false, typeof (Boolean));

        private static readonly WorldstateSymbol HappyFalse2 = new WorldstateSymbol
            (WorldProperties.Happy, false, typeof (Boolean));

        private static readonly WorldstateSymbol ToyTrue = new WorldstateSymbol
            (WorldProperties.HasToy, true, typeof (Boolean));

        private static readonly WorldstateSymbol ToyFalse = new WorldstateSymbol
            (WorldProperties.HasToy, false, typeof (Boolean));

        private static readonly Node V1 = new Node
            (new List<WorldstateSymbol> {HappyTrue}, new List<WorldstateSymbol>(), 1);

        private static readonly Node V2 = new Node
            (new List<WorldstateSymbol> {HappyFalse1}, new List<WorldstateSymbol>(), 1);

        private static readonly Node V3 = new Node
            (new List<WorldstateSymbol> {HappyFalse2}, new List<WorldstateSymbol>(), 1);

        private static readonly Node V4 = new Node
            (new List<WorldstateSymbol> {ToyTrue}, new List<WorldstateSymbol> {HappyFalse2}, 1);

        private static readonly Node V5 = new Node
            (new List<WorldstateSymbol> {ToyFalse}, new List<WorldstateSymbol> {HappyFalse1}, 1);

        private static readonly Node V6 = new Node
            (new List<WorldstateSymbol> {ToyFalse}, new List<WorldstateSymbol> {HappyFalse2}, 1);

        private static readonly Node V7 = new Node
            (new List<WorldstateSymbol> {ToyFalse}, new List<WorldstateSymbol> {HappyFalse2}, 2);


        [Test]
        public void ConstructorTest() {
            Assert.NotNull(V1);
        }

        /// <summary>
        ///     Equality is depending on goal values, current values and heuristic
        /// </summary>
        [Test]
        public void EqualityTest() {
            Assert.True(V2.Equals(V3));
            Assert.AreEqual(V2, V3);
            Assert.AreEqual(V5, V6);
        }

        [Test]
        public void NoEqualityTest() {
            Assert.AreNotEqual(V1, V2);
            Assert.AreNotEqual(V4, V5);
            Assert.AreNotEqual(V6, V7);
            Assert.AreNotEqual(V3, null);
        }

        [Test]
        public void EqualityOperatorTest() {
            Assert.True(V2 == V3);
            Assert.True(V3 == V2);
            Assert.True(V1 != V3);
            Assert.True(V2 != V4);
        }
    }

}