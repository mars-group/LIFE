using System;
using DalskiAgent.Movement.Movers;
using LifeAPI.Environment;
using LifeAPI.Layer;
using LifeAPI.Spatial;
using LifeAPI.Spatial.Shape;


namespace DalskiAgent.Agents {

  /// <summary>
  ///   This agent is part of a spatial world. It has a position and 
  ///   is registered at an environment to provide collision detection. 
  ///   Per default it is immobile but it can be equipped with a movement module.
  /// </summary>
  public abstract class SpatialAgent : Agent, ISpatialEntity {

    private readonly IShape _shape;      // Shape describing this agent's body.
    private readonly IEnvironment _env;  // IESC implementation for collision detection.
    protected AgentMover Mover;          // Class for agent movement. 


    /// <summary>
    ///   Instantiate a new agent with spatial data. Only available for specializations.
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="env">Environment implementation reference.</param> 
    /// <param name="shape">Shape describing this agent's body.</param>
    /// <param name="pos">The initial position. If null, it is tried to be set randomly.</param>
    /// <param name="dir">Direction of the agent. If null, then 0°.</param>
    protected SpatialAgent(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, 
                           IEnvironment env, IShape shape = null, 
                           TVector pos = default(TVector), Direction dir = null) : 
      
      // Create the base agent.
      base(layer, regFkt, unregFkt) {

      /*** SpatialAgent additions: ***/
      
      // Check, if the agent already has a direction and a form. If not, create a cube facing north.
      if (dir == null) dir = new Direction();
      if (shape == null) shape = new Quad(new TVector(1.0, 1.0, 1.0), pos, dir);

      // Place the agent in the environment.
      bool success;
      if (!pos.IsNull()) success = env.Add(this, pos, dir.GetDirectionalVector().GetTVector());
      else success = env.AddWithRandomPosition(this, TVector.Origin, env.MaxDimension, env.IsGrid);    
      if (!success) throw new Exception("[SpatialAgent] Agent placement in environment failed (ESC returned 'false')!");
      
      // Save references for later use.
      _shape = shape;
      _env = env;
    }


    /// <summary>
    ///   This function unbinds the agent from the environment.
    ///   It is triggered by the base agent, when alive flag is 'false'.
    /// </summary>
    protected override void Remove() {
      base.Remove();
      _env.Remove(this);
    }


    //-------------------------------------------------------------------------
    // GET methods. The direction is probably not used.

    /// <summary>
    ///   Returns the position of the agent.
    /// </summary>
    /// <returns>A position vector.</returns>
    public TVector GetPosition() {
      return _shape.GetPosition();
    }


    /// <summary>
    ///   Returns the agent's heading.
    /// </summary>
    /// <returns>A direction vector.</returns>
    public Direction GetDirection() {
      return _shape.GetRotation();
    }


    public Enum GetInformationType() {
      throw new NotImplementedException();
    }

    public IShapeOld Shape { get; set; }  //TODO not so fine here ...
    public Enum GetCollisionType() {
      throw new NotImplementedException();
    }
  }
}
