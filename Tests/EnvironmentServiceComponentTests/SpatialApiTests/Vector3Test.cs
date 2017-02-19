using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using NUnit.Framework;

namespace EnvironmentServiceComponentTests.SpatialApiTests {

  public class Vector3Test {

    [Test]
    public void TestGetDistance() {
      Assert.IsTrue(Vector3.GetDistance(Vector3.Up, Vector3.Zero) - 1 < double.Epsilon);
      Assert.IsTrue(Vector3.GetDistance(Vector3.Zero, Vector3.Zero) < double.Epsilon);
    }
  }
}