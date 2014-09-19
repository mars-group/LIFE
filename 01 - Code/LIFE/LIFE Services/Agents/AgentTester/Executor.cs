using System;
using System.Threading;
using CommonTypes.DataTypes;
using ESCTestLayer.Implementation;
using GenericAgentArchitecture.Movement;
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
      var environment = AgentBuilder.CreateWolvesScenarioEnvironment(); //CreateRandomMovingAgents(2, 10, 10);//
      new Executor(environment).Run(850);
      
      /*
      Console.WriteLine ("Testmethode für die Bewegungsklasse.");      
      var m = new GridMovement (new ESC(), 0, new Vector3f(1, 1, 0));     
      m.MoveToPosition(new Vector2f(2, 0), 2);
      Console.ReadLine();*/
    }
  }
}