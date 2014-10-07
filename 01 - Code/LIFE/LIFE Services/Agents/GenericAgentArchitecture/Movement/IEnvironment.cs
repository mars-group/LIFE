using GenericAgentArchitecture.Agents;
using LayerAPI.Interfaces;

namespace GenericAgentArchitecture.Movement {
  
  /// <summary>
  ///   This interface declares functions needed for  movement services.
  ///   It thereby enables abstraction from ESC specific methods.
  /// </summary>
  public interface IEnvironment : IGenericDataSource {


    /// <summary>
    ///   Add a new agent to the environment.
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    /// <param name="data">Container with movement data.</param>
    void AddAgent(SpatialAgent agent, MData data);


    /// <summary>
    ///   Remove an agent from the environment.
    /// </summary>
    /// <param name="agent">The agent to delete.</param>
    void RemoveAgent(SpatialAgent agent);


    /// <summary>
    ///   Update the position and heading of an agent. This function is also
    ///   responsible to set the new values to the agent movement container. 
    /// </summary>
    /// <param name="agent">The agent to move.</param>
    /// <param name="position">New position.</param>
    /// <param name="direction">New heading.</param>
    void ChangePosition(SpatialAgent agent, Vector position, Direction direction);
  }
}
