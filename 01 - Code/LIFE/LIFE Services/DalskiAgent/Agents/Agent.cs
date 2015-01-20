using System;
using DalskiAgent.Reasoning;
using LifeAPI.Agent;
using LifeAPI.Layer;

namespace DalskiAgent.Agents {

  /// <summary>
  /// The abstract agent. This is the most generic agent form, it specifies the main execution
  /// cycle and several extension points available for specialized agent implementations.
  /// </summary>
  public abstract class Agent : IAgent {
    
    private readonly ILayer _layerImpl;               // Layer reference needed for delegate calls. 
    private readonly UnregisterAgent _unregFkt;       // Delegate for unregistration function.
    protected IAgentLogic ReasoningComponent;         // The agent's reasoning logic.         
    protected bool IsAlive;                           // Alive flag for execution and deletion checks.
    public readonly long Id;                          // Unique identifier. 


    /// <summary>
    /// Constructor for an abstract agent. It serves as a base class that is extended with
    /// domain specific sensors, actions and reasoning, optionally containing a knowledge base.  
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    protected Agent(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt) {
      IsAlive = true;
      Id = GetNewId();
      if (this is IAgentLogic) ReasoningComponent = (IAgentLogic) this;  
      regFkt(layer, this);
      _layerImpl = layer;
      _unregFkt = unregFkt;
    }


    /// <summary>
    /// This is the main function of the agent program. It executes all three steps, 
    /// calling the concrete functions of the domain-specific agent, respectively.
    /// The execution is governed by some external runtime manager. 
    /// </summary>
    public void Tick() {
      Sense();                                         // Phase 1: Perception
      var action = ReasoningComponent.Reason();        // Phase 2: Reasoning      
      if (IsAlive && action != null) action.Execute(); // Phase 3: Execution
      else if (!IsAlive) Remove();                     // Agent deletion.      
    }


    /// <summary>
    ///   Returns the current simulation tick.
    /// </summary>
    /// <returns>Execution tick counter value.</returns>
    protected long GetTick() {
      return _layerImpl.GetCurrentTick();
    }


    /// <summary>
    ///   The removal method stops external triggering of the agent. It is designed
    ///   to be overridden by more specific methods calling down to this via 'base'. 
    /// </summary>
    protected virtual void Remove() {
      _unregFkt(_layerImpl, this);
    }


    /// <summary>
    ///   Base method for sensing phase. It is called in each Tick() first and may 
    ///   be implemented by concrete agents.
    /// </summary>
    protected virtual void Sense() {}


    /// <summary>
    /// Default debug output. It may be overwritten by more concrete functions.
    /// </summary>
    /// <returns>Console output string.</returns>
    public new virtual string ToString() {
      return "Agent: " + Id + "\t  Tick: " + GetTick();
    }


    /// <summary>
    ///   Get or set an agent identifier.
    /// </summary>
    public Guid ID { get; set; }
  


  
    //_____________________________________________________________________________________________
    // Auxiliary stuff for colored, synchronized console output and human-readable agent IDs.

    private static readonly object Lock = new object(); // Lock object for output and ID dist.
    private static long _idCounter;                     // Counter for agent ID distribution.    

    /// <summary>
    ///   Print a (colored) message synchronized to the console window.
    /// <param name="message">The message to display.</param>
    /// <param name="color">The message color (default: gray).</param>
    /// </summary>
    protected static void PrintMessage(string message, ConsoleColor color = ConsoleColor.Gray) {
      lock(Lock) {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.Gray;          
      }     
    }
  

    /// <summary>
    ///   Return an ID for a new agent.
    /// </summary>
    /// <returns>A unique identifier.</returns>
    private static long GetNewId() {
      lock (Lock) {
        return _idCounter++;
      }
    }
  }
}
