using GenericAgentArchitecture.Perception;
using GenericAgentArchitectureCommon.Interfaces;
using LayerAPI.Interfaces;


namespace GenericAgentArchitecture.Agents {

  /// <summary>
  /// The abstract agent. This is the most generic agent form, it specifies the main execution
  /// cycle and several extension points available for specialized agent implementations.
  /// </summary>
  public abstract class Agent : IAgent {

    public readonly long Id;                           // Unique identifier.
    public long Cycle { get; protected set; }          // The current execution cycle.   
    protected readonly PerceptionUnit PerceptionUnit;  // Sensor container and input gathering. 
    protected readonly IAgentLogic ReasoningComponent; // The agent's reasoning logic.     


    /// <summary>
    /// Constructor for an abstract agent. It serves as a base class that is extended with
    /// domain specific sensors, actions and reasoning, optionally containing a knowledge base.  
    /// </summary>
    /// <param name="id">A unique identifier, shall be used for log and communication.</param>
    protected Agent(long id) {
      Id = id;
      PerceptionUnit = new PerceptionUnit();
      if (this is IAgentLogic) ReasoningComponent = (IAgentLogic) this;    
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
      Cycle ++;  
    }
 

    /// <summary>
    /// Default debug output. It may be overwritten by more concrete functions.
    /// </summary>
    /// <returns>Console output string.</returns>
    public new virtual string ToString() {
      return "Agent: " + Id + "\t  Cycle: " + Cycle;
    }
  }
}