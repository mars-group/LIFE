using System;
using System.Linq;
using DalskiAgent.Agents;
using DalskiAgent.Reasoning;
using LifeAPI.Layer;
using SpatialAPI.Environment;

namespace WolvesModel.Agents {
  
  /// <summary>
  ///   This class is responsible for the creation of additional agents.
  /// </summary>
  internal class AgentSpawner : Agent, IAgentLogic {

    // Normally, you don't save this references. But in this case we need them to pass along.
    private readonly ILayer _layer;             // Layer reference needed for delegate calls.
    private readonly RegisterAgent _regFkt;     // Agent registration function pointer.
    private readonly UnregisterAgent _unregFkt; // Delegate for unregistration function.
    private readonly IEnvironment _env;         // Environment reference.
    private readonly Random _random;            // Random number generator for agent spawning.


    /// <summary>
    ///   Create a new agent spawner. It is capable of sense the 
    ///   other agents and can create some based on their counts. 
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="env">Environment implementation reference.</param> 
    public AgentSpawner(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IEnvironment env) : 
      base(layer, regFkt, unregFkt) {
      _layer = layer;
      _regFkt = regFkt;
      _unregFkt = unregFkt;
      _env = env;
      _random = new Random();
    }


    /// <summary>
    ///   Agent spawning logic.
    /// </summary>
    /// <returns>Always 'null', because this agent does not interact.</returns>
    public IInteraction Reason() {

      // Output numbers.
      var grass = _env.ExploreAll().OfType<Grass>().Count();

      // Grass spawning.
      var create = _random.Next(50 + grass*2) < 20;
      if (create) {
        var g = new Grass(_layer, _regFkt, _unregFkt, _env);
        PrintMessage("["+GetTick()+"] Neues Gras auf Position "+g.GetPosition(), ConsoleColor.Cyan);
      }
      return null;
    }
  }
}
