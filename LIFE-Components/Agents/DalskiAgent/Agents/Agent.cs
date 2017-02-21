using System;
using ASC.Communication.ScsServices.Service;
using LIFE.API.Agent;
using LIFE.API.Layer;
using LIFE.Components.Agents.DalskiAgent.Interactions;
using LIFE.Components.Agents.DalskiAgent.Perception;

namespace LIFE.Components.Agents.DalskiAgent.Agents {

  /// <summary>
  /// The abstract agent. This is the most generic agent form, it specifies the main execution
  /// cycle and several extension points available for specialized agent implementations.
  /// </summary>
	public abstract class Agent : AscService, IAgent {
 
    private readonly UnregisterAgent _unregFkt;   // Delegate for unregistration function.
    private readonly int _executionGroup;

    /// <summary>Sensor aggregation unit and data storage.</summary>
    protected readonly SensorArray SensorArray;
     
    /// <summary>Alive flag for execution and deletion checks.</summary>
    protected bool IsAlive;

    /// <summary>Layer reference needed for delegate calls.</summary>
    public readonly ILayer Layer;


    /// <summary>
    /// Constructor for an abstract agent. It serves as a base class that is extended with
    /// domain specific sensors, actions and reasoning, optionally containing a knowledge base.  
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="id">Fixed GUID to use in this agent (optional).</param>
    /// <param name="executionGroup">
    ///   The execution group of your agent:
    ///   0 : execute never
    ///   1 : execute every tick
    ///   n : execute every tick where tick % executionGroup == 0
    /// </param>
    protected Agent(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, byte[] id = null, int executionGroup = 1) {
      if (id == null) ID = Guid.NewGuid();
      else ID = ID = new Guid(id);
      _executionGroup = executionGroup;
      IsAlive = true;
      SensorArray = new SensorArray();
      regFkt(layer, this, executionGroup);
      Layer = layer;
      _unregFkt = unregFkt;
    }


    /// <summary>
    /// Sensing phase. Calls all sensors per default.
    /// </summary>
    protected virtual void Sense() {
      SensorArray.SenseAll();       
    }


	  /// <summary>
	  /// Abstract implementation requires sub-class to implement it.
	  /// </summary>
	  /// <returns>The interaction the agent shall execute.</returns>
	  protected abstract IInteraction Reason();


    /// <summary>
    /// This is the main function of the agent program. It executes all three steps, 
    /// calling the concrete functions of the domain-specific agent, respectively.
    /// The execution is governed by some external runtime manager. 
    /// </summary>
    public void Tick() {
      try {
        if (!IsAlive) Remove();
        
        // Phase 1: Perception
        Sense(); 

        // Phase 2: Reasoning  
		    var action = Reason();
        
        // Phase 3: Execution
        if (IsAlive && action != null) action.Execute(); 
        else if (!IsAlive) Remove(); // Agent deletion. 
      }
      catch (Exception e) {
        Console.Error.WriteLine(e);
        throw;
      }     
    }


    /// <summary>
    ///   Returns the current simulation tick.
    /// </summary>
    /// <returns>Execution tick counter value.</returns>
    public long GetTick() {
      return Layer.GetCurrentTick();
    }


    /// <summary>
    ///   The removal method stops external triggering of the agent. It is designed
    ///   to be overridden by more specific methods calling down to this via 'base'. 
    /// </summary>
    protected virtual void Remove() {
      _unregFkt(Layer, this, _executionGroup);
    }


    /// <summary>
    /// Default debug output. It may be overwritten by more concrete functions.
    /// </summary>
    /// <returns>Console output string.</returns>
    public override string ToString() {
      return "["+ID+" "+GetType().Name+"]\t\t Tick: " + GetTick();
    }


      /// <summary>
      ///   Get or set an agent identifier.
      /// </summary>
      public Guid ID { get; set; }
	}
}