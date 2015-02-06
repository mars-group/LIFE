using System;
using DalskiAgent.Movement.Movers;
using LifeAPI.Layer;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Environment;
using SpatialAPI.Shape;


namespace DalskiAgent.Agents {

  /// <summary>
  ///   This agent is part of a spatial world. It has a position and 
  ///   is registered at an environment to provide collision detection. 
  ///   Per default it is immobile but it can be equipped with a movement module.
  /// </summary>
  public abstract class SpatialAgent : Agent, ISpatialEntity {

    public IShape Shape { get; set; }      // Shape describing this agent's body.
    private readonly IEnvironment _env;    // IESC implementation for collision detection.
    protected AgentMover Mover;            // Class for agent movement. 


    /// <summary>
    ///   Instantiate a new agent with spatial data. Only available for specializations.
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="env">Environment implementation reference.</param> 
    /// <param name="shape">Shape describing this agent's body and initial parameters.</param>
    /// <param name="minPos">Minimum position for random placement [default: origin].</param>
    /// <param name="maxPos">Maximal random position [default: environmental extent].</param>
    protected SpatialAgent(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IEnvironment env, 
                           IShape shape = null, Vector3 minPos = default(Vector3), Vector3 maxPos = default(Vector3)) : 
      
      // Create the base agent. Per default it is collidable.
      base(layer, regFkt, unregFkt) {
      CollisionType = SpatialAPI.Entities.Movement.CollisionType.MassiveAgent;      

      // Place the agent in the environment.
      bool success;
      if (shape != null) {
        Shape = shape;
        success = env.Add(this, Shape.Position, Shape.Rotation);
      }
      
      // If the agent has no shape yet, create a cube facing north and add at a random position. 
      else {   
        Shape = new Cuboid(new Vector3(1.0, 1.0, 1.0), new Vector3(), new Direction());
        if (minPos.IsNull()) minPos = Vector3.Zero;
        if (maxPos.IsNull()) maxPos = env.MaxDimension;
        success = env.AddWithRandomPosition(this, minPos, maxPos, env.IsGrid); 
      }
      
      if (!success) throw new Exception("[SpatialAgent] Agent placement in environment failed (ESC returned 'false')!");
      _env = env;  // Save environment reference.       
    }




    /// <summary>
    ///   This function unbinds the agent from the environment.
    ///   It is triggered by the base agent, when alive flag is 'false'.
    /// </summary>
    protected override void Remove() {
      base.Remove();
      _env.Remove(this);
    }


    //_________________________________________________________________________
    // GET methods. The direction is probably not used.

    /// <summary>
    ///   Returns the position of the agent.
    /// </summary>
    /// <returns>A position vector.</returns>
    public Vector3 GetPosition() {
      return Shape.Position;
    }


    /// <summary>
    ///   Returns the dimension of this agent's bounding box.
    /// </summary>
    /// <returns>A dimension vector.</returns>
    public Vector3 GetDimension() {
      return Shape.Bounds.Dimension;
    }


    /// <summary>
    ///   Returns the agent's heading.
    /// </summary>
    /// <returns>A direction vector.</returns>
    public Direction GetDirection() {
      return Shape.Rotation;
    }


    /// <summary>
    ///   This agent's collision type.
    /// </summary>
    public Enum CollisionType { get; protected set; }


    /// <summary>
    ///   The information type of this agent (e.g. its classname).
    /// </summary>
    public Enum InformationType { get; protected set; }
  }
}
