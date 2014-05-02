using System;
using System.Collections.Generic;
using System.Threading;
using Primitive_Architecture.Agents;
using Primitive_Architecture.Interfaces;

namespace Primitive_Architecture.Dummies {
  
  /// <summary>
  ///   This is the execution environment, responsible for periodic triggering of all agents.
  /// </summary>
  internal class Executor {

    private readonly List<ITickClient> _clients; // A list of all clients to execute.


    /// <summary>
    ///   Instantiate a runtime.
    /// <param name="clients">A list of executable (tickable) clients.</param>
    /// </summary>
    private Executor(List<ITickClient> clients) {
      _clients = clients;
    }


    /// <summary>
    ///   Execution routine. All agents are ticked in a sequential order.
    /// </summary>
    /// <param name="delay">Thread delay (in ms), 0 for manual execution.</param>
    private void Run(int delay) {
      while (true) {
        //TODO: Arbitrary execution or some user input handling would be nice.
        foreach (var client in _clients) client.Tick();
        
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
      var agents = AgentBuilder.CreateHeatingScenarioAgents();
      new Executor(agents).Run(0);
      Console.ReadLine();
    }
  }
}