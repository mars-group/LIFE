using GenericAgentArchitecture.Environments;
using GenericAgentArchitecture.Movement;
using GenericAgentArchitecture.Movement.Movers;

namespace GenericAgentArchitecture.Agents {

  /// <summary>
  ///   This agent is part of a spatial world. It has a position and 
  ///   is registered at an environment to provide collision detection. 
  ///   Per default it is immobile but it can be equipped with a movement module.
  /// </summary>
  public abstract class SpatialAgent : Agent {

    private readonly IEnvironment _env;    // IESC implementation for collision detection.
    protected readonly MovementData Data;  // Container for position, direction and speeds.
    protected AgentMover Mover;            // Class for agent movement. 


    /// <summary>
    ///   Instantiate a new agent with spatial data. Only available for specializations.
    /// </summary>
    /// <param name="id">Unique agent identifier.</param>
    /// <param name="env">Environment implementation reference.</param>
    /// <param name="pos">The initial position. If null, it is tried to be set randomly.</param>
    protected SpatialAgent(long id, IEnvironment env, Vector pos) : base(id) {
      _env = env;
      Data = _env.AddAgent(this, pos); // Enlist the agent in environment.
    }


    /// <summary>
    ///   This function unbinds the agent from the environment.
    ///   It is intended to be called by an interaction method.
    /// </summary>
    protected void Remove() {
      _env.RemoveAgent(this);
    } 


    //-------------------------------------------------------------------------
    // GET methods. The latter two are probably not used.

    /// <summary>
    ///   Returns the position of the agent.
    /// </summary>
    /// <returns>A position vector.</returns>
    public Vector GetPosition() {
      return new Vector(Data.Position.X, Data.Position.Y, Data.Position.Z);
    }


    /// <summary>
    ///   Returns the agent's heading.
    /// </summary>
    /// <returns>A direction vector.</returns>
    public Vector GetDirection() {
      return Data.Direction.GetDirectionalVector();
    }


    /// <summary>
    ///   Returns the dimension of an agent.
    /// </summary>
    /// <returns>A vector representing the bounding box around an agent.</returns>
    public Vector GetDimension() {
      return new Vector(Data.Dimension.X, Data.Dimension.Y, Data.Dimension.Z);
    }
  }
}
