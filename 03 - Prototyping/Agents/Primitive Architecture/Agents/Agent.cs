using System;
using Primitive_Architecture.Dummies;
using Primitive_Architecture.Interactions;
using Primitive_Architecture.Interfaces;
using Primitive_Architecture.Perception;
using Environment = Primitive_Architecture.Dummies.Environment;

namespace Primitive_Architecture.Agents {

  /// <summary>
  /// The abstract agent. This is the most generic agent form, it specifies the main execution
  /// cycle and several extension points available for specialized agent implementations.
  /// </summary>
  internal abstract class Agent : ITickClient {

    private long _cycle;                               // The current execution cycle.
    protected readonly string Id;                      // Unique identifier.
    protected readonly PerceptionUnit PerceptionUnit;  // Sensor container and input gathering. 
    protected IAgentLogic ReasoningComponent;          // The agent's reasoning logic.
    protected readonly bool DebugEnabled;              // Controls console debug output.
    public readonly InteractionContainer Interactions; // Repertoire of all interactions.  
    public Vector Position;                            // Position in an environment.


    /// <summary>
    /// Constructor for an abstract agent. It serves as a base class that is extended with
    /// domain specific sensors, actions and reasoning, optionally containing a knowledge base.  
    /// </summary>
    /// <param name="id">A unique identifier, shall be used for log and communication.</param>
    protected Agent(string id) {
      Id = id;
      DebugEnabled = true; 
      PerceptionUnit = new PerceptionUnit();
      if (this is IAgentLogic) ReasoningComponent = (IAgentLogic) this;
      Interactions = new InteractionContainer (this, Environment.IACLoader);  
    }


    /// <summary>
    /// This is the main function of the agent program. It executes all three steps, 
    /// calling the concrete functions of the domain-specific agent, respectively.
    /// The execution is governed by some external runtime manager. 
    /// </summary>
    public void Tick() {
      PerceptionUnit.SenseAll();                // Phase 1: Perception
      var action = ReasoningComponent.Reason(); // Phase 2: Reasoning
      if (action != null) action.Execute();     // Phase 3: Execution
      _cycle ++;

      // Print the runtime information for debug purposes. 
      if (DebugEnabled) Console.WriteLine(ToString());    
    }
 

    /// <summary>
    /// Default debug output. It may be overwritten by more concrete functions.
    /// </summary>
    /// <returns>Console output string.</returns>
    protected new virtual string ToString() {
      var pos = "";
      if (Position != null) pos = " Position: "+Position;
      return "Agent: " + Id + "   Cycle: " + _cycle + pos;
    }
  }
}