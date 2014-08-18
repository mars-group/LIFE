using System;
using System.Threading;
using ESCTestLayer;
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
      var esc       = new ESC();
      var position  = new Vector3f(2, 0, 0);
      var direction = new Vector3f(1, 1, 1);
      var dimension = new Vector3f(1, 1, 1);


      var aabb = ESC.GetAABB(position, direction, dimension);
      Console.WriteLine("\nX-Inv: "+aabb.XIntv.ToString());
      Console.WriteLine("Y-Inv: "+aabb.YIntv.ToString());
      Console.WriteLine("Z-Inv: "+aabb.ZIntv.ToString());  */


      var esc = new ESC();
      var max = new Vector3f(4, 3, 0);
      const bool itg = false;

      esc.Register(0, new Vector3f(1, 1, 0));
      esc.Register(1, new Vector3f(1, 1, 0));
      esc.Register(2, new Vector3f(1, 1, 0));
      esc.Register(3, new Vector3f(1, 1, 0));

      esc.SetRandomPosition(0, null, max, itg);
      esc.SetRandomPosition(1, null, max, itg);
      esc.SetRandomPosition(2, null, max, itg);
      esc.SetRandomPosition(3, null, max, itg);



      Console.ReadLine();
    }
  }
}