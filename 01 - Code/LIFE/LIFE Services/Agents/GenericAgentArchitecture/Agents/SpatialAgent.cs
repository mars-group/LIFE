using GenericAgentArchitecture.Movement;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Agents {
  
  /// <summary>
  ///   This agent is part of a spatial world. It has a position and is registered at an 
  ///   Environment Service Component (ESC) to provide collision detection. 
  ///   Per default it is immobile but it can be equipped with a movement module.
  /// </summary>
  public abstract class SpatialAgent : Agent {

    private readonly IEnvironment _env;  // IESC implementation for collision detection.
    protected readonly MData Data;       // Container for position, direction and speeds.
    protected readonly AgentMover Mover; // Class for agent movement. 


    /// <summary>
    ///   Instantiate a new agent with spatial data. Only available for specializations.
    /// </summary>
    /// <param name="id">Unique agent identifier.</param>
    /// <param name="esc">Environment implementation reference.</param>
    /// <param name="pos">The initial position.</param>
    /// <param name="dim">Agent's physical dimension.</param>
    /// <param name="type">Type of agent. Needed for ESC reconnaissance.</param>
    /// <param name="phys">'True' if the agent is collidable.</param>
    protected SpatialAgent(long id, IEnvironment esc, TVector pos, TVector dim, int type, bool phys = true) : base(id) {
      _env = esc;
      Data = new MData(pos);
      Mover = null;   
      _env.AddAgent(this, Data); // Enlist the agent in environment.
    }


    /// <summary>
    ///   When the agent is destroyed, it is no longer physically present. Remove it from ESC!
    /// </summary>
    ~SpatialAgent () {
      if (_env != null) _env.RemoveAgent(this);
    }


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
