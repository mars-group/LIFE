using System;
using LIFE.Components.ESC.Implementation;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;
using NUnit.Framework;

namespace EnvironmentServiceComponentTests.EscTests {

  internal class TestEscWithDistribution {

    private IEnvironment _esc;

    [SetUp]
    public void Init() {
      _esc = new DistributedESC(maxLeafObjectCount: 1000);
    }


    public static double GetDecimalDegreesByMeters(double distanceInMeters, double distanceOfOneArcSecond = 31.1) {
      var arcSeconds = distanceInMeters/distanceOfOneArcSecond;
      return arcSeconds/60/60;
    }

    [Test]
    public void TestExplore() {
      var wp = new ElefantSpatialEntity {
        AgentGuid = Guid.NewGuid(),
        Shape = new Sphere(new Vector3(32.112, 0, -25.112), GetDecimalDegreesByMeters(5))
      };
      _esc.Add(wp, new Vector3(32.112, 0, -25.112));
      var radius = GetDecimalDegreesByMeters(19200);

      var result = _esc.Explore(new Sphere(new Vector3(32.111, 0, -25.111), radius));
      Assert.IsTrue(result.GetEnumerator().MoveNext());
      //foreach (var r in result) { Console.WriteLine($"Explore found {r.AgentType} with Position Lat: {r.Shape.Position.X} , Lon: {r.Shape.Position.Z}"); }
    }

    private class ElefantSpatialEntity : ISpatialEntity {
      public Guid AgentGuid { get; set; }

      public Type AgentType { get; set; }

      public Enum CollisionType => null;

      public IShape Shape { get; set; }
    }
  }
}