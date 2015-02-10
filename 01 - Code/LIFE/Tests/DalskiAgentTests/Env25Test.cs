using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DalskiAgent.Auxiliary.Environment;
using NUnit.Framework;
using SpatialAPI.Entities.Transformation;

namespace DalskiAgentTests {

  [TestFixture]
  public class Env25Test {


    [Test]
    public void Env25PerformanceTest() {
      const int envWidth  = 1000;
      const int envHeight = 1000;
      const int nrAgents  = 100000;

      // Create environment and desired number of spatial entities (unpositioned, 1x1 dimension).
      Env25 env = new Env25(envWidth, envHeight);
      Parallel.For(0, nrAgents, i1 => env.AddWithRandomPosition(new Obj(new Float2(0, 0), new Float2(1, 1))));
    }


    [Test]  // Do a number of insertion tests to measure general performance.
    public void PerformanceTest() {
          
      // Test series.
      int[] tests = {1000, 2000, 4000, 8000, 16000};
      const int envWidth  = 1000;  // Important: Ensure that width*height
      const int envHeight = 1000;  // is sufficient to store max. number of agents!

      Stopwatch stopwatch = Stopwatch.StartNew();

      // Perform tests and output elapsed time.
      for (int t = 0; t < tests.Length; t++) {
        Env25 env = new Env25(envWidth, envHeight);
        for (int i = 0; i < tests[t]; i++) {
          Assert.True(env.Add(new Obj(new Float2(0, 0), new Float2(0.9f, 0.9f)), new Vector3(i%envWidth, (int)(i/envWidth))));
        } 
        Console.WriteLine("["+tests[t]+" agents]: "+stopwatch.ElapsedMilliseconds + " ms");
        stopwatch.Restart();  // Reset stopwatch.
      }        
    }



    //_____________________________________________________________________________________________
    // Test of collision detection functions.


    // First rectangle.
    readonly Float2 _posR1  = new Float2(0, 0);
    readonly Float2 _span   = new Float2(1, 1);


    [Test]  // No collision: 'False' expected!
    public void TestCollision_noCollision() {
      Assert.False(CDF.IntersectRects(_posR1, _span, new Float2(2,  2), _span));
      Assert.False(CDF.IntersectRects(_posR1, _span, new Float2(-1, -2), _span));
    }



    [Test]  // Rectangles touch: 'False' expected!
    public void TestCollision_touches() {
      Assert.False(CDF.IntersectRects(_posR1, _span, new Float2(1,  0), _span));
      Assert.False(CDF.IntersectRects(_posR1, _span, new Float2(1,  1), _span));
      Assert.False(CDF.IntersectRects(_posR1, _span, new Float2(0, -1), _span));
    }



    [Test]  // Standard collision case, where two rectangles overlap: 'True' expected!  
    public void TestCollision_overlaps() {

      // Overlap in +x direction.
      Assert.True(CDF.IntersectRects(_posR1, _span, new Float2(0.8f, 0), _span));

      // Overlap in +x, +y direction.
      Assert.True(CDF.IntersectRects(_posR1, _span, new Float2(0.8f, 0.9f), _span));

      // Overlap in -x, -y direction.
      Assert.True(CDF.IntersectRects(_posR1, _span, new Float2(-0.8f, -0.9f), _span));
    }



    [Test]  // "Contains" is treated like a collision: 'True' expected!
    public void TestCollision_contains() {
      
      // Rectangles are equal.
      Assert.True(CDF.IntersectRects(_posR1, _span, _posR1, _span));
      
      // R1 contains R2.
      Assert.True(CDF.IntersectRects(_posR1, _span, new Float2(0.25f, 0.25f), new Float2(0.5f, 0.5f)));

      // R1 is contained by R2.
      Assert.True(CDF.IntersectRects(new Float2(0.25f, 0.25f), new Float2(0.5f, 0.5f), _posR1, _span));
    }
  }
}
