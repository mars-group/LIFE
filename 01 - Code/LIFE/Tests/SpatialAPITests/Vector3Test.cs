using NUnit.Framework;
using SpatialAPI.Entities.Transformation;

namespace SpatialTests {


    public class Vector3Test {
        private const double Epsilon = 0.00001;

        [Test]
        public void TestGetDistance() {
            Assert.IsTrue(Vector3.GetDistance(Vector3.Up, Vector3.Zero) - 1 < Epsilon);
            Assert.IsTrue(Vector3.GetDistance(Vector3.Zero, Vector3.Zero) < Epsilon);
        }
    }

}