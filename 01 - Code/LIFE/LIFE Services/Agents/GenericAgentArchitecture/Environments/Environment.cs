using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GenericAgentArchitecture.Agents;
using LayerAPI.Interfaces;

namespace GenericAgentArchitecture.Environments {
  
  /// <summary>
  ///   Base class for an environment. This is an agent container with several access options.
  /// </summary>
  public abstract class Environment : ITickClient {

    private long _idCounter;                    // This counter is needed for agent ID distribution.
    private int _execCounter;                   // Pointer to the selected agent during execution.
    private List<Agent> _execList;              // Execution schedule (list optionally shuffled).
    private readonly List<Agent> _agents;       // The agents living in this environment.
    protected readonly Random Random;           // Random number generator.
    protected long Cycle { get; private set; }  // Counter for execution cycle.
    public bool PrintInformation { get; set; }  // Controls debug information output.
    public bool RandomExecution  { get; set; }  // Flag to set random or sequential execution. 


    /// <summary>
    ///   Creation of base class. Initializes the agent list and sets sequential execution.
    /// </summary>
    protected Environment() {
      _agents = new List<Agent>();
      _execList = new List<Agent>();
      Random = new Random();
      Cycle = 0;
      _idCounter = 0;
    }


    /// <summary>
    ///   The environment's Tick() advances the simulation by one step.
    /// </summary>
    public void Tick() {
      // First, we advance the environment.
      AdvanceEnvironment();

      // If a random execution is desired, we shuffle the agent list.
      if (RandomExecution) {
        _execList = new List<Agent>(_agents);
        for (var i = 0; i < _agents.Count; i++) {
          var j = Random.Next(i, _agents.Count);
          var temp = _execList[i];
          _execList[i] = _execList[j];
          _execList[j] = temp;
        }
      }

      // Finally, the agents are executed.
      for (_execCounter = 0; _execCounter < _agents.Count; _execCounter++) {
        _execList[_execCounter].Tick();
      }

      // Debug output wished? If so, print it now!
      if (PrintInformation) PrintEnvironment();
      Cycle ++;
    }


    /// <summary>
    ///   Add an agent to the execution list.
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    protected void AddAgent(Agent agent) {
      _agents.Add(agent);
      _execList.Add(agent);
    }


    /// <summary>
    ///   Remove an agent from the execution list.
    /// </summary>
    /// <param name="agent">The agent to remove.</param>
    protected void RemoveAgent(Agent agent) {
      if (_execList.IndexOf(agent) <= _execCounter) _execCounter --;
      _agents.Remove(agent);
      _execList.Remove(agent);    
    }


    /// <summary>
    ///   Get all agents that are contained in this environment.
    /// </summary>
    /// <returns>A read-only list of all available agents.</returns>
    protected IEnumerable<Agent> GetAllAgents() {
      return new ReadOnlyCollection<Agent>(_agents);
    }


    /// <summary>
    ///   Get a new agent ID and increase the counter.
    /// </summary>
    /// <returns>A unused ID.</returns>
    public long GetNewID() {
      return _idCounter++;
    }


    /// <summary>
    ///   This function allows execution of environment-specific code.
    /// </summary>
    protected abstract void AdvanceEnvironment();


    /// <summary>
    ///   Console output function. Prints the environment and all agent logs.
    /// </summary>
    protected virtual void PrintEnvironment() {}
  }
}