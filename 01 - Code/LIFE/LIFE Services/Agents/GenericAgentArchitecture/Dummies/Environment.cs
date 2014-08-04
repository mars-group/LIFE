using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Interfaces;

namespace GenericAgentArchitecture.Dummies {
  
  /// <summary>
  ///   Base class for an environment. This is an agent container with several access options.
  /// </summary>
  internal abstract class Environment : ITickClient {

    protected readonly List<Agent> Agents;     // The agents living in this environment.
    protected long Cycle { get; private set; } // Counter for execution cycle.
    public bool PrintInformation { get; set; } // Controls debug information output.
    public bool RandomExecution { get; set; }  // Flag to set random or sequential execution. 
    public static IIacLoader IacLoader { get; private set; } // Interaction loader reference. 


    /// <summary>
    ///   Creation of base class. Initializes the agent list and sets sequential execution.
    /// </summary>
    /// <param name="interactions">The domain specific interaction loader.</param>
    protected Environment(IIacLoader interactions) {
      Agents = new List<Agent>();
      Cycle = 0;
      IacLoader = interactions;
    }


    /// <summary>
    ///   The environment's Tick() advances the simulation by one step.
    /// </summary>
    public void Tick() {
      // First, we advance the environment.
      AdvanceEnvironment();

      // This second list is needed for steadiness during execution (deletion resilience). 
      var execList = new List<Agent>(Agents);

      // If a random execution is desired, we shuffle the agent list.
      if (RandomExecution) {
        var rnd = new Random();
        for (var i = 0; i < Agents.Count; i++) {
          var j = rnd.Next(i, execList.Count);
          var temp = execList[i];
          execList[i] = execList[j];
          execList[j] = temp;
        }
      }

      // Finally, the agents are executed.
      foreach (var agent in execList) {
        agent.Tick();
      }

      // Debug output wished? If so, print it now!
      if (PrintInformation) PrintEnvironment();
      Cycle ++;
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
    public IEnumerable<Agent> GetAllAgents() {
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


    /// <summary>
    ///   Console output function. Prints the environment and all agent logs.
    /// </summary>
    protected virtual void PrintEnvironment() {}
  }
}