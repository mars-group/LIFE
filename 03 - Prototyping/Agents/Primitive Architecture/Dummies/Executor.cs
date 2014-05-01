using System;
using System.Collections.Generic;
using Primitive_Architecture.Agents;
using Primitive_Architecture.Agents.Heating;
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
    private void Run() {
      var input = "";
      while (input != "q") {
        //TODO: Arbitrary execution or some user input handling would be nice.
        foreach (var client in _clients) client.Tick();
        input = Console.ReadLine();
      }
    }


    /// <summary>
    ///   Program entry. Creates some agents and starts them.
    /// </summary>
    public static void Main() {
      var agents = AgentBuilder.CreateHeatingScenarioAgents();
      new Executor(agents).Run();
      Console.ReadLine();
    }
  }
}