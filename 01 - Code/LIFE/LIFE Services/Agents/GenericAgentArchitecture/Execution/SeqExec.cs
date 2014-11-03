using System;
using System.Collections.Generic;
using System.Threading;
using DalskiAgent.Agents;
using DalskiAgent.Auxiliary;
using DalskiAgent.Environments;

namespace DalskiAgent.Execution {
  
  /// <summary>
  ///   This class periodicly triggers all agents in a sequential order.
  ///   It only exists for very simple testing reasons and should not be used otherwise!
  ///   Use the LayerExecution instead - that is the wrapper for MARS LC registration. 
  /// </summary>
  public class SeqExec : IExecution {
    
    private int _execCounter;                // Pointer to the selected agent during execution.
    private IEnvironment _environment;       // Environmental reference for optional execution. 
    private List<Agent> _execList;           // Execution schedule (list optionally shuffled).
    private readonly List<Agent> _agents;    // The agents living in this environment.
    private readonly bool _randomExecution;  // Flag to set random or iterative execution. 
    private readonly Random _random;         // Random number generator.
    private long _cycle;                     // Counter for execution cycle.  
    private long _idCounter;                 // This counter is needed for agent ID distribution.


    //TODO ID automatisch erhöhen! (Braucht nicht mehr von außen geschehen!)
    // Macht Agent im Konstruktor per Abfrage von hier.


    /// <summary>
    ///   Instantiate a sequential agent executor.
    /// </summary>
    /// <param name="randomExec">Flag to set random or iterative execution.</param>
    public SeqExec(bool randomExec) {
      _randomExecution = randomExec;
      _agents = new List<Agent>();
      _execList = new List<Agent>();
      _random = new Random();
      _idCounter = 0;
      _cycle = 0;     
    }


    /// <summary>
    ///   The Tick() function advances all agents by one step.
    ///   It is called by main execution routine (below).
    /// </summary>
    private void Tick() {      

      // Execute environment first (if set).
      if (_environment != null) _environment.AdvanceEnvironment();

      // If a random execution is desired, we shuffle the agent list.
      if (_randomExecution) {
        _execList = new List<Agent>(_agents);
        for (var i = 0; i < _agents.Count; i++) {
          var j = _random.Next(i, _agents.Count);
          var temp = _execList[i];
          _execList[i] = _execList[j];
          _execList[j] = temp;
        }
      }

      // Finally, the agents are executed.
      for (_execCounter = 0; _execCounter < _agents.Count; _execCounter++) {
        _execList[_execCounter].Tick();
      }
      _cycle ++;
    }


    /// <summary>
    ///   Execution routine. Sends a tick to the environment container.
    /// </summary>
    /// <param name="delay">Thread delay (in ms), 0 for manual execution.</param>
    /// <param name="view">The console view module.</param>
    public void Run(int delay, ConsoleView view) {
      while (true) {
        Tick();
        if (view != null) view.Print();

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
    ///   Add an agent to the execution list.
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    public void AddAgent(Agent agent) {
      _agents.Add(agent);
      _execList.Add(agent);
    }


    /// <summary>
    ///   Remove an agent from the execution list.
    /// </summary>
    /// <param name="agent">The agent to remove.</param>
    public void RemoveAgent(Agent agent) {
      if (_execList.IndexOf(agent) <= _execCounter) _execCounter --;
      _agents.Remove(agent);
      _execList.Remove(agent);    
    }


    /// <summary>
    ///   Return an ID for a new agent.
    /// </summary>
    /// <returns>A unique identifier.</returns>
    public long GetNewID() {
      return _idCounter++;
    }


    /// <summary>
    ///   Return the current simulation tick.
    /// </summary>
    /// <returns>Current tick counter value.</returns>
    public long GetCurrentTick() {
      return _cycle;
    }


    /// <summary>
    ///   Set an environment reference.
    ///   If set, an AdvanceEnvironment() function is executed each tick.
    /// </summary>
    /// <param name="env">The environment to execute.</param>
    public void SetEnvironment(IEnvironment env) {
      _environment = env;
    }
  }
}
