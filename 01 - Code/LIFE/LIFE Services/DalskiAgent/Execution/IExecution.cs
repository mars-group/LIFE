using DalskiAgent.Agents;

namespace DalskiAgent.Execution {
  
  /// <summary>
  ///   This interface defines base functionality needed for agent execution.
  ///   The base agent needs an implementation of this interface and adds /
  ///   removes itself automatically.
  /// </summary>
  public interface IExecution {

    /// <summary>
    ///   Add an agent to the execution system.
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    void AddAgent(Agent agent);
    

    /// <summary>
    ///   Remove an agent from the execution system.
    /// </summary>
    /// <param name="agent">The agent to remove.</param>    
    void RemoveAgent(Agent agent);


    /// <summary>
    ///   Return an ID for a new agent.
    /// </summary>
    /// <returns>A unique identifier.</returns>
    long GetNewID();


    /// <summary>
    ///   Return the current simulation tick.
    /// </summary>
    /// <returns>Current tick counter value.</returns>
    long GetCurrentTick();
  }
}
