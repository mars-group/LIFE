using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Primitive_Architecture.Agents;
using Primitive_Architecture.Interactions.Wolves;
using Primitive_Architecture.Interfaces;

namespace Primitive_Architecture.Dummies {
  
  /// <summary>
  ///   Base class for an environment. This is an agent container with several access options.
  /// </summary>
  internal abstract class Environment : ITickClient {

    protected readonly List<Agent> Agents;     // The agents living in this environment.
    public bool RandomExecution { get; set; }  // Flag to set random or sequential execution. 
    public static IIACLoader IACLoader { get; private set; } // Interaction loader reference. 


    /// <summary>
    ///   Creation of base class. Initializes the agent list and sets sequential execution.
    /// </summary>
    /// <param name="interactions">The domain specific interaction loader.</param>
    protected Environment(IIACLoader interactions) {
      Agents = new List<Agent>();
      IACLoader = interactions;
    }


    /// <summary>
    ///   The environment's Tick() advances the simulation by one step.
    /// </summary>
    public void Tick() {
      // First, we advance the environment.
      AdvanceEnvironment();

      // If a random execution is desired, we shuffle the agent list.
      if (RandomExecution) {
        var rnd = new Random();
        for (var i = 0; i < Agents.Count; i++) {
          var j = rnd.Next(i, Agents.Count);
          var temp = Agents[i];
          Agents[i] = Agents[j];
          Agents[j] = temp;
        }
      }

      // Finally, the agents are executed.
      foreach (var agent in Agents) {
        agent.Tick();
      }
    }


    /// <summary>
    ///   Add an agent to the execution list.
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    public virtual void AddAgent(Agent agent) {
      Agents.Add(agent);
    }


    /// <summary>
    ///   Remove an agent from the execution list.
    /// </summary>
    /// <param name="agent">The agent to remove.</param>
    public void RemoveAgent(Agent agent) {
      Agents.Remove(agent);
    }


    /// <summary>
    ///   Get all agents that are contained in this environment.
    /// </summary>
    /// <returns>A read-only list of all available agents.</returns>
    public IReadOnlyList<Agent> GetAllAgents() {
      return new ReadOnlyCollection<Agent>(Agents);
    }


    /// <summary>
    ///   This function allows execution of environment-specific code.
    /// </summary>
    protected abstract void AdvanceEnvironment();


    /// <summary>
    ///   Calculate a distance between two agents. This can be an arbitrary metric.
    /// </summary>
    /// <param name="x">The first agent.</param>
    /// <param name="y">The second agent.</param>
    /// <returns>A value describing the distance between these two agents.</returns>
    public abstract double GetDistance(Agent x, Agent y);
  }
}