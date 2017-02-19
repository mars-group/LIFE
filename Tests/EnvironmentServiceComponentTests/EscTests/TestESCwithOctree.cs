using System.Linq;
using EnvironmentServiceComponentTests.Entities;
using LIFE.Components.ESC.Implementation;
using LIFE.Components.ESC.SpatialAPI.Common;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialObjectTree;
using NUnit.Framework;

namespace EnvironmentServiceComponentTests.EscTests {

  public class TestEscWithOctree : TestEsc {

    [SetUp]
    public void Init() {
      Esc = new TreeESC(new SpatialObjectOctree<ISpatialEntity>(new Vector3(25, 25, 25), 1));
    }

    [Test]
    public void MoveOutOfBoundaryAndBeBlocked() {
      var boundary = new Vector3(10, 10);
      IEnvironment esc = new TreeESC(null, new BoundarySpecification(EnvBoundaryType.BlockOnExit, boundary));
      ISpatialEntity a1 = new TestSpatialEntity(Vector3.One);
      Assert.True(esc.Add(a1, Vector3.Zero));
      Assert.True(esc.Move(a1, new Vector3(9, 9)).Success);
      Assert.True(esc.Move(a1, new Vector3(1, 1)).Success);

      var movementResult = esc.Move(a1, new Vector3(1, 1));
      Assert.False(movementResult.Success);
      Assert.True(movementResult.Collisions.First().GetType() == typeof(MovementBlockingBoundaryEntity));
    }
  }
}