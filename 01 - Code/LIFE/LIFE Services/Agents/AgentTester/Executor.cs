using System;
using System.Threading;
using ESCTestLayer;
using GenericAgentArchitecture.Dummies;
using LayerAPI.Interfaces;

namespace AgentTester {
  
  /// <summary>
  ///   This class periodicly triggers the environment and thereby all agents.
  /// </summary>
  internal class Executor {
    private readonly ITickClient _environment; // The agent container.


    /// <summary>
    ///   Instantiate a runtime.
    ///   <param name="environment">The environment to execute.</param>
    /// </summary>
    private Executor(ITickClient environment) {
      _environment = environment;
    }


    /// <summary>
    ///   Execution routine. Sends a tick to the environment container.
    /// </summary>
    /// <param name="delay">Thread delay (in ms), 0 for manual execution.</param>
    private void Run(int delay) {
      while (true) {
        _environment.Tick();

        // Manual or automatic execution.
        if (delay == 0) Console.ReadLine();
        else {
          Console.WriteLine();
          Thread.Sleep(delay);
        }
      }
      // ReSharper disable once FunctionNeverReturns
    }


    /// <summary>
    ///   Program entry. Creates some agents and starts them.
    /// </summary>
    public static void Main() {
      //var environment = AgentBuilder.CreateRandomMovingAgents(2, 10, 10);//CreateWolvesScenarioEnvironment();
      //new Executor(environment).Run(850);

      /*
      var esc = new ESC();
      esc.Register(1, new Vector3f(1, 1, 1));
      esc.Register(2, new Vector3f(1, 1, 1));
      esc.Register(3, new Vector3f(1, 1, 1));
      esc.Register(4, new Vector3f(1, 1, 1));
      
      Vector3f min = new Vector3f(0, 0, 0);
      Vector3f max = new Vector3f(2, 2, 0);
      bool integer = true;


      Console.WriteLine(esc.SetRandomPosition(1, min, max, integer));
      Console.WriteLine(esc.SetRandomPosition(2, min, max, integer));
      Console.WriteLine(esc.SetRandomPosition(3, min, max, integer));
      Console.WriteLine(esc.SetRandomPosition(4, min, max, integer));
      *//*

      var f1 = new Float3(2, 1, 1);
      var f2 = f1.GetNormalVector();
      Console.WriteLine(f2.X+"   "+f2.Y+"   "+f2.Z);
      Console.WriteLine(.GetLength());
      */

      Float3 nr1, nr2, nr3;
      var flt = new Float3(0.5f, 0.5f, 0);
      nr1 = flt.GetNormalVector();
      nr1.GetPlanarOrthogonalVectors(out nr2, out nr3);

      Console.WriteLine(nr1.X+"   "+nr1.Y+"   "+nr1.Z+"  (lng: "+nr1.GetLength()+")");
      Console.WriteLine(nr2.X+"   "+nr2.Y+"   "+nr2.Z+"  (lng: "+nr2.GetLength()+")");
      Console.WriteLine(nr3.X+"   "+nr3.Y+"   "+nr3.Z+"  (lng: "+nr3.GetLength()+")");
      Console.WriteLine();

      
      var p1 = new Float3(1, 1, 0);

      var p2x = p1.X*nr1.X + p1.X*nr2.X + p1.X*nr3.X;
      var p2y = p1.Y*nr1.Y + p1.Y*nr2.Y + p1.Y*nr3.Y;
      var p2z = p1.Z*nr1.Z + p1.Z*nr2.Z + p1.Z*nr3.Z;

      var p2 = new Float3(p2x, p2y, p2z);


      Console.WriteLine(p1.X+"   "+p1.Y+"   "+p1.Z+"  (lng: "+p1.GetLength()+")");
      Console.WriteLine(p2.X+"   "+p2.Y+"   "+p2.Z+"  (lng: "+p2.GetLength()+")");
      


      Console.ReadLine();
    }
  }
}