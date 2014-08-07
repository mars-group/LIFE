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


      var esc = new ESC();
      esc.Register(1, new Vector3f(1, 1, 1));
      esc.Register(2, new Vector3f(1, 1, 1));
      esc.Register(3, new Vector3f(1, 1, 1));
      esc.Register(4, new Vector3f(1, 1, 1));
      
      Vector3f min = new Vector3f(0, 0, 0);
      Vector3f max = new Vector3f(5, 5, 0);
      bool integer = true;


      Console.WriteLine(esc.SetRandomPosition(1, min, max, integer));
      Console.WriteLine(esc.SetRandomPosition(2, min, max, integer));
      Console.WriteLine(esc.SetRandomPosition(3, min, max, integer));
      Console.WriteLine(esc.SetRandomPosition(4, min, max, integer));



      Console.ReadLine();
    }
  }
}