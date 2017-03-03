using System;
using System.Collections.Generic;
using ASC.Communication.ScsServices.Service;
using LIFE.API.Agent;
using LIFE.API.Layer;
using LIFE.Components.Agents.BasicAgents.Perception;
using LIFE.Components.Agents.BasicAgents.Reasoning;

/* The following warnings are useless, because this is an abstract base class
 * and we don't know if the user maybe want to use a variable or overwrite it. */
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace LIFE.Components.Agents.BasicAgents.Agents {

  /// <summary>
  /// The abstract agent. This is the most generic agent form, it specifies the main execution
  /// cycle and several extension points available for specialized agent implementations.
  /// </summary>
  public abstract class Agent : AscService, IAgent {

    private readonly UnregisterAgent _unregFkt;              // Delegate for unregistration function.
    protected readonly SensorArray SensorArray;              // Sensor aggregation unit and data storage.
    protected bool IsAlive;                                  // Alive flag for execution and deletion checks.
    protected int ExecutionGroup;                            // Agent execution frequency.
    protected readonly ILayer Layer;                         // Layer reference needed for delegate calls.
    protected readonly Dictionary<string, object> AgentData; // Dictionary for arbitrary result values.
    public Guid ID { get; set; }                             // The agent identifier [required by LifeAPI].



    /// <summary>
    /// Constructor for an abstract agent. It serves as a base class that is extended with
    /// domain specific sensors, actions and reasoning, optionally containing a knowledge base.  
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="id">Fixed GUID to use in this agent (optional).</param>
    /// <param name="freq">Agent execution frequency (ticks). [default: 1]</param>
    protected Agent(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, byte[] id=null, int freq=1) {
      if (id == null) ID = Guid.NewGuid();
      else ID = ID = new Guid(id);
      ExecutionGroup = freq;
      IsAlive = true;
      SensorArray = new SensorArray();
      regFkt(layer, this, freq);
      Layer = layer;
      _unregFkt = unregFkt;
      AgentData = new Dictionary<string, object>();
    }


    /// <summary>
    /// This is the main function of the agent program. It executes all three steps, 
    /// calling the concrete functions of the domain-specific agent, respectively.
    /// The execution is governed by some external runtime manager. 
    /// </summary>
    public void Tick() {
      try {
        if (!IsAlive) Remove();                          // Agent deletion.
        Sense();                                         // Phase 1: Perception
		    var action = Reason();                           // Phase 2: Reasoning
        if (IsAlive && action != null) action.Execute(); // Phase 3: Execution
        else if (!IsAlive) Remove();                     // Agent deletion.
      }
      catch (Exception ex) {
        Console.Error.WriteLine(ex);
        throw;
      }     
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
    ///   Returns the current simulation tick.
    /// </summary>
    /// <returns>Execution tick counter value.</returns>
    protected long GetTick() {
      return Layer.GetCurrentTick();
    }


    /// <summary>
    ///   The removal method stops external triggering of the agent. It is designed
    ///   to be overridden by more specific methods calling down to this via 'base'. 
    /// </summary>
    protected virtual void Remove() {
      _unregFkt(Layer, this, ExecutionGroup);
    }


    /// <summary>
    /// Default debug output. It may be overwritten by more concrete functions.
    /// </summary>
    /// <returns>Console output string.</returns>
    public override string ToString() {
      return "["+ID+" "+GetType().Name+"]\t\t Tick: " + GetTick();
    }
	}
}