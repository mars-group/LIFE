using System;
using System.Threading;
using AgentTester.Wolves.Agents;
using GenericAgentArchitecture.Auxiliary;
using LayerAPI.Interfaces;

namespace AgentTester {
  
  /// <summary>
  ///   This class periodicly triggers the environment and thereby all agents.
  /// </summary>
  internal class Executor {
    
    private readonly ITickClient _environment; // The agent container.
    private readonly ConsoleView _view;        // The console view module.


    /// <summary>
    ///   Instantiate a runtime.
    ///   <param name="environment">The environment to execute.</param>
    ///   <param name="view">The console view module.</param>
    /// </summary>
    private Executor(ITickClient environment, ConsoleView view) {
      _environment = environment;
      _view = view;
    }


    /// <summary>
    ///   Execution routine. Sends a tick to the environment container.
    /// </summary>
    /// <param name="delay">Thread delay (in ms), 0 for manual execution.</param>
    private void Run(int delay) {
      while (true) {
        _environment.Tick();
        if (_view != null) _view.Print();

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
      var environment = AgentBuilder.CreateWolvesScenarioEnvironment(20, 10, 5);
      var view = AgentBuilder.CreateWolvesView((Grassland) environment);
      new Executor(environment, view).Run(0);
    }
  }
}


/* Delegate-Zuweisungsarten:
 * 1) Statische Funktion machen, Zeiger = Funktionsname: 
 *    public static ConsoleColor Fkt (SpatialAgent agt) { ... }
 *    GetColor = new GetColor(Fkt)
 *    GetColor = Fkt   // kürzer!
 * 
 * 2) Anonyme Funktion erstellen:
 *    GetColor = delegate(SpatialAgent agt) { ... }
 */
