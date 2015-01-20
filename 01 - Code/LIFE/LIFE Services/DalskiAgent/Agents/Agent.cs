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


    /// <summary>
    /// Constructor for an abstract agent. It serves as a base class that is extended with
    /// domain specific sensors, actions and reasoning, optionally containing a knowledge base.  
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    protected Agent(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt) {
      IsAlive = true;
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
      var action = ReasoningComponent.Reason();        // Phase 2: Reasoning      
      if (IsAlive && action != null) action.Execute(); // Phase 3: Execution
      else if (!IsAlive) Remove();                     // Agent deletion.      
    }


    /// <summary>
    ///   The removal method stops external triggering of the agent. It is designed
    ///   to be overridden by more specific methods calling down to this via 'base'. 
    /// </summary>
    protected virtual void Remove() {
      _unregFkt(_layerImpl, this);
    }


    /// <summary>
    ///   Returns the current simulation tick.
    /// </summary>
    /// <returns>Execution tick counter value.</returns>
    public long GetTick() {
      return _layerImpl.GetCurrentTick();
    }


    /// <summary>
    /// Default debug output. It may be overwritten by more concrete functions.
    /// </summary>
    /// <returns>Console output string.</returns>
    public new virtual string ToString() {
      return "Agent: " + ID + "\t  Tick: " + GetTick();
    }


    /// <summary>
    ///   Get or set an agent identifier.
    /// </summary>
    public Guid ID { get; set; }
  
  
    /******************** Console output stuff. ***********************/
    private static readonly object Obj = new object();     // Lock object for LC output.
    
    /// <summary>
    ///   Print a (colored) message synchronized to the console window.
    /// <param name="message">The message to display.</param>
    /// <param name="color">The message color (default: gray).</param>
    /// </summary>
    public static void PrintMessage(string message, ConsoleColor color = ConsoleColor.Gray) {
      lock(Obj) {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.Gray;          
      }     
    }  
  }
}
