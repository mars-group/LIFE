using System.Collections.Generic;
using DalskiAgent.Agents;
using DalskiAgent.Movement;
using LayerAPI.Interfaces;

namespace DalskiAgent.Environments {
  
  /// <summary>
  ///   This interface declares functions needed for movement services.
  ///   It thereby enables abstraction from ESC specific methods.
  /// </summary>
  public interface IEnvironment : IGenericDataSource {


    /// <summary>
    ///   Add a new agent to the environment.
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    /// <param name="pos">The agent's initial position.</param>
    /// <param name="mdata">The movement data container reference.</param>
    void AddAgent(SpatialAgent agent, Vector pos, out MovementData mdata);


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


    /// <summary>
    ///   Retrieve all agents of this environment.
    /// </summary>
    /// <returns>A list of all spatial agents.</returns>
    List<SpatialAgent> GetAllAgents();
    //TODO Later, we should generalize this method to entities. 
    // It is most likely that not everything in the environment is an agent!


    /// <summary>
    ///   This method allows execution of environment related code. 
    ///   It is only useful in sequential mode and is executed before any agent.
    ///   For layer execution, inherit layer from ITickClient and register at itself!
    /// </summary>
    void AdvanceEnvironment();
  }
}
