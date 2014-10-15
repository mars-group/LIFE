using System.Collections.Generic;
using GoapCommon.Interfaces;
using GoapGraphConnector.SimpleGraph;
using GoapModelTest.Worldstates;
using NUnit.Framework;

namespace GoapTests {
    [TestFixture]
    public class CustomGaphNodeTest {

        private static readonly IGoapWorldProperty HappyTrue = new IsHappy(true);
        private static readonly IGoapWorldProperty HappyFalse1 = new IsHappy(false);
        private static readonly IGoapWorldProperty HappyFalse2 = new IsHappy(false);
        private static readonly IGoapWorldProperty ToyTrue = new HasToy(true);
        private static readonly IGoapWorldProperty ToyFalse = new HasToy(false);

        private static readonly Node V1 = new Node(new List<IGoapWorldProperty> { HappyTrue }, new List<IGoapWorldProperty>(),1);
        private static readonly Node V2 = new Node(new List<IGoapWorldProperty> { HappyFalse1 }, new List<IGoapWorldProperty>(), 1);
        private static readonly Node V3 = new Node(new List<IGoapWorldProperty> { HappyFalse2 }, new List<IGoapWorldProperty>(), 1);
        private static readonly Node V4 = new Node(new List<IGoapWorldProperty> { ToyTrue }, new List<IGoapWorldProperty> { HappyFalse2 }, 1);

        private static readonly Node V5 = new Node(new List<IGoapWorldProperty> { ToyFalse }, new List<IGoapWorldProperty> { HappyFalse1 }, 1);
        private static readonly Node V6 = new Node(new List<IGoapWorldProperty> { ToyFalse }, new List<IGoapWorldProperty> { HappyFalse2 }, 1);

        private static readonly Node V7 = new Node(new List<IGoapWorldProperty> { ToyFalse }, new List<IGoapWorldProperty> { HappyFalse2 }, 2);

       
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