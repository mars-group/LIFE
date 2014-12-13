using DalskiAgent.Execution;
using DalskiAgent.Perception;
using DalskiAgent.Reasoning;
using LifeAPI.Agent;

namespace DalskiAgent.Agents {

  /// <summary>
  /// The abstract agent. This is the most generic agent form, it specifies the main execution
  /// cycle and several extension points available for specialized agent implementations.
  /// </summary>
  public abstract class Agent : IAgent {

    private readonly IExecution _execution;            // Execution reference for add/remove and queries.     
    protected readonly PerceptionUnit PerceptionUnit;  // Sensor container and input gathering. 
    protected IAgentLogic ReasoningComponent;          // The agent's reasoning logic.         
    protected bool IsAlive;                            // Alive flag for execution and deletion checks.
    public readonly long Id;                           // Unique identifier. 

    /// <summary>
    /// Constructor for an abstract agent. It serves as a base class that is extended with
    /// domain specific sensors, actions and reasoning, optionally containing a knowledge base.  
    /// </summary>
    /// <param name="exec">Execution container reference.</param>
    protected Agent(IExecution exec) {
      _execution = exec;
      Id = exec.GetNewID();
      IsAlive = true;
      PerceptionUnit = new PerceptionUnit();
      if (this is IAgentLogic) ReasoningComponent = (IAgentLogic) this;       
    }


    /// <summary>
    /// This is the main function of the agent program. It executes all three steps, 
    /// calling the concrete functions of the domain-specific agent, respectively.
    /// The execution is governed by some external runtime manager. 
    /// </summary>
    public void Tick() {
      PerceptionUnit.SenseAll();                       // Phase 1: Perception
      var action = ReasoningComponent.Reason();        // Phase 2: Reasoning      
      if (IsAlive && action != null) action.Execute(); // Phase 3: Execution
      else if (!IsAlive) Remove();                     // Agent deletion.      
    }


    /// <summary>
    ///   This function registers the agent at the execution component. Later functions
    ///   may add more instructions. This function has to be called manually as last  
    ///   statement in the specific agent constructor. 
    /// </summary>
    protected void Init() {
      _execution.AddAgent(this);
    }


    /// <summary>
    ///   The removal method stops external triggering of the agent. It is designed
    ///   to be overridden by more specific methods calling down to this via 'base'. 
    /// </summary>
    protected virtual void Remove() {
      _execution.RemoveAgent(this);
    }


    /// <summary>
    ///   Returns the current simulation tick.
    /// </summary>
    /// <returns>Execution tick counter value.</returns>
    public long GetTick() {
      return _execution.GetCurrentTick();
    }


    /// <summary>
    /// Default debug output. It may be overwritten by more concrete functions.
    /// </summary>
    /// <returns>Console output string.</returns>
    public new virtual string ToString() {
      return "Agent: " + Id + "\t  Cycle: " + GetTick();
    }
  }
}