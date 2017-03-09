using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnvironmentServiceComponentTests.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using NUnit.Framework;
// ReSharper disable AccessToModifiedClosure

namespace EnvironmentServiceComponentTests.EscTests {

  [Ignore("Ignore a fixture")]
  [TestFixture]
  public class TestParallelization {

    private readonly Random _random = new Random();
    private readonly int _tickCount = 100;

    private int Next(int max = 10) {
      return _random.Next(max);
    }


    private void AddAndMoveNewEntity(IESC esc, int i) {
      var entity = new TestSpatialEntity(i, new Vector3(1, 1, 1));
      esc.AddWithRandomPosition(entity, Vector3.Null, new Vector3(Next(100), Next(100), Next(100)), true);
      for (var tick = 0; tick < _tickCount; tick++) {
        var vector = new Vector3(Next(), Next(), Next());
        esc.Move(entity, vector);
      }
    }

    [Test]
    public void TestParallelAddAndMove() {
      foreach (var esc in EnvironmentManager.GetAll()) {
        var esc1 = esc;
        var tasks = new List<Task>();
        for (var i = 0; i < 40000; i++) tasks.Add(Task.Factory.StartNew(() => AddAndMoveNewEntity(esc1, i)));
        Task.WaitAll(tasks.ToArray());
        Console.WriteLine("All threads complete for " + esc);
      }
    }
  }
}