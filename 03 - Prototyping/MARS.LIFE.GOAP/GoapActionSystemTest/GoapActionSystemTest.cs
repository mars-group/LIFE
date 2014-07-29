using System;
using System.Collections.Generic;
using GoapActionSystem.Implementation;
using GoapCommon.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoapActionSystemTest
{
    [TestClass]
    public class GoapActionSystemTest
    {
        [TestMethod]
        public void TestMethod1() {
            Assert.IsFalse(false);
        }
    }

    [TestClass]
    public class PlannerTest {

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPlannerConstructorWithEmptyActionList() {
            new GoapPlanner(1, new List<IGoapAction>());
        }
    }
}
