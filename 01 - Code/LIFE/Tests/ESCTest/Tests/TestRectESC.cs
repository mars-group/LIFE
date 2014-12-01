using EnvironmentServiceComponent.Implementation;
using ESCTest.Entities;
using NUnit.Framework;
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

        protected override ISpatialEntity GenerateAgent(double x, double y)
        {
            return new RectAgent(x, y);
        }
    }

}