﻿using System.Collections.Generic;
using GoapCommon.Interfaces;
using GoapGraphConnector.CustomGraph;
using GoapModelTest.Worldstates;
using NUnit.Framework;

namespace GoapTests {
    [TestFixture]
    public class CustomGaphVertexTest {

        private static readonly IGoapWorldstate HappyTrue = new Happy(true);
        private static readonly IGoapWorldstate HappyFalse1 = new Happy(false);
        private static readonly IGoapWorldstate HappyFalse2 = new Happy(false);
        private static readonly IGoapWorldstate ToyTrue = new HasToy(true);
        private static readonly IGoapWorldstate ToyFalse = new HasToy(false);

        private static readonly Vertex V1 = new Vertex(new List<IGoapWorldstate> { HappyTrue }, 1, "v1_happy_true");
        private static readonly Vertex V2 = new Vertex(new List<IGoapWorldstate> { HappyFalse1 }, 1, "v2_happy_false");
        private static readonly Vertex V3 = new Vertex(new List<IGoapWorldstate> { HappyFalse2 }, 1, "v3_happy_false");
        private static readonly Vertex V4 = new Vertex(new List<IGoapWorldstate> { ToyTrue }, 1, "v4_toy_true");
        private static readonly Vertex V5 = new Vertex(new List<IGoapWorldstate> { ToyFalse }, 1, "v5_toy_false");

        private static readonly Vertex V6 = new Vertex(new List<IGoapWorldstate>());
        private static readonly Vertex V7 = new Vertex(new List<IGoapWorldstate>());
        
        [Test]
        public void ConstructorThreeParamTest() {
            Assert.NotNull(V1);
        }

        [Test]
        public void ConstructorOneParamTest() {
            Assert.NotNull(V7);
        }

        /// <summary>
        ///     Equality is depending on type of wordstates and their bool value
        /// </summary>
        [Test]
        public void EqualityTest() {
            Assert.AreEqual(V2, V3);
            Assert.AreEqual(V6, V7);
        }

        [Test]
        public void NoEqualityTest() {
            Assert.AreNotEqual(V1, V2);
            Assert.AreNotEqual(V4, V5);
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