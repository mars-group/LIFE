﻿using System.Collections.Generic;
using GoapCommon.Interfaces;
using GoapGraphConnector.SimpleGraph;
using GoapModelTest.Actions;
using GoapModelTest.Worldstates;
using NUnit.Framework;

namespace GoapTests {
    [TestFixture]
    internal class CustomGraphEdgeTest {

        private static readonly Node V1 = new Node(new List<IGoapWorldProperty> { new IsHappy(true) }, new List<IGoapWorldProperty>(), 1);
        private static readonly Node V2 = new Node(new List<IGoapWorldProperty> { new IsHappy(true) }, new List<IGoapWorldProperty> { new HasToy(false)}, 1);

        private static readonly Node V3 = new Node(new List<IGoapWorldProperty> { new HasToy(false) }, new List<IGoapWorldProperty>(), 1);
        private static readonly Node V4 = new Node(new List<IGoapWorldProperty> { new HasMoney(true) }, new List<IGoapWorldProperty> { new HasToy(false) }, 1);

       


        private static readonly Edge E1 = new Edge(new ActionClean(), V1, V2);
        private static readonly Edge E2 = new Edge(new ActionClean(), V1, V2);
        private static readonly Edge E3 = new Edge(new ActionGetToy(), V2, V3);
        private static readonly Edge E4 = new Edge(new ActionPlay(), V2, V4);
        
        [Test]
        public void SourceVertexTest() {
            Assert.AreEqual(E1.GetSource(),V1);
        }

        [Test]
        public void TargetVertexTest() {
            Assert.AreEqual(E1.GetTarget(),V2);    
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