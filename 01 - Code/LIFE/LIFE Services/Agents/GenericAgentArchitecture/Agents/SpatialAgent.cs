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

    protected readonly IESC Environment;   // IESC implementation for collision detection.
    protected readonly MData Position;     // Container for position, direction and speeds.


    /// <summary>
    ///   Instantiate a new agent with spatial data. Only available for specializations.
    /// </summary>
    /// <param name="id">Unique agent identifier.</param>
    /// <param name="esc">IESC implemenation reference.</param>
    /// <param name="pos">The initial position.</param>
    /// <param name="dim">Agent's physical dimension.</param>
    /// <param name="type">Type of agent. Needed for ESC reconnaissance.</param>
    /// <param name="phys">'True' if the agent is collidable.</param>
    protected SpatialAgent(long id, IESC esc, TVector pos, TVector dim, int type, bool phys = true) : base(id) {
      Environment = esc;
      Position = new MData(pos);

      // Enlist the agent and place it at its current position. 
      Environment.Add((int) id, type, phys, dim);
      Environment.SetPosition((int) id, pos, TVector.UnitVectorXAxis);
    }


    /// <summary>
    ///   When the agent is destroyed, it is no longer physically present. Remove it from ESC!
    /// </summary>
    ~SpatialAgent () {
      if (Environment != null) Environment.Remove((int) ID);
    }
  }
}
