using ESCTestLayer.Interface;
using GenericAgentArchitecture.Movement;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Agents {
  
  /// <summary>
  ///   This agent is part of a spatial world. It has a position and is registered at an 
  ///   Environment Service Component (ESC) to provide collision detection. 
  ///   Per default it is immobile but it can be equipped with a movement module.
  /// </summary>
  public abstract class SpatialAgent : Agent {

    public readonly IESC Environment;               // IESC implementation for collision detection.
    public Vector Position { get; protected set; }  // The agent's center (current position). 
    public float  Pitch    { get; protected set; }  // Direction (lateral axis).
    public float  Yaw      { get; protected set; }  // Direction (vertical axis).


    /// <summary>
    ///   Instantiate a new agent with spatial data. Only available for specializations.
    /// </summary>
    /// <param name="id">Unique agent identifier.</param>
    /// <param name="esc">IESC implemenation reference.</param>
    /// <param name="pos">The initial position.</param>
    /// <param name="dim">Agent's physical dimension.</param>
    /// <param name="type">Type of agent. Needed for ESC reconnaissance.</param>
    /// <param name="phys">'True' if the agent is collidable.</param>
    protected SpatialAgent(long id, IESC esc, Float3 pos, Float3 dim, int type, bool phys = true) : base(id) {
      Environment = esc;
      Position = new Vector(pos.X, pos.Y, pos.Z);
      Pitch = 0.0f;  // Default facing is straight line northbound. 
      Yaw = 0.0f;    // May be overwritten in specific constructor.

      // Enlist the agent and place it at its current position. 
      Environment.Add((int) id, type, phys, new TVector(dim.X, dim.Y, dim.Z));
      Environment.SetPosition((int) id, new TVector(pos.X, pos.Y, pos.Z), TVector.UnitVectorXAxis);
    }


    /// <summary>
    ///   When the agent is destroyed, it is no longer physically present. Remove it from ESC!
    /// </summary>
    ~SpatialAgent () {
      if (Environment != null) Environment.Remove((int) ID);
    }
  }


  /// <summary>
  ///   This structure just holds three floats.
  /// </summary>
  public struct Float3 {
    public float X, Y, Z;
  }
}
