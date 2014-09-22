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
      //TODO ACHTUNG! Entfernungsangaben der Agenten fehlerhaft (Reichweite bis über 18 statt 8). Umwelt schuld?
      //var environment = AgentBuilder.CreateWolvesScenarioEnvironment();
      //new Executor(environment).Run(850);
      
      
      Console.WriteLine ("Testmethode für die Bewegungsklasse.");      
      var esc = new ESC();
      esc.Add(1, new Vector(1, 1, 0));
      esc.SetPosition(1, new Vector(1, 4), new Vector(1, 0));
      var m = new GridMovement(esc, 11, new Vector(1, 1));
      m.MoveToPosition(new Vector(1, 4), 5);
      Console.ReadLine();
    }
  }
}