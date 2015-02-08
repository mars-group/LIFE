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
  public abstract class SpatialAgent : Agent, ISpatialAgent {

    private readonly IEnvironment _env; // IESC implementation for collision detection.
    protected AgentMover Mover; // Class for agent movement. 


    /// <summary>
    ///   Spatial entity describing this agent's body.
    /// </summary>
    public ISpatialEntity Entity { get; private set; }


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
    /// <param name="collisionType">Agent collision type [default: 'MassiveAgent'].</param>
    protected SpatialAgent(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IEnvironment env,
      IShape shape = null, Vector3 minPos = default(Vector3), Vector3 maxPos = default(Vector3),
      Enum collisionType = null) :

        // Create the base agent. 
        base(layer, regFkt, unregFkt) {

      // Set up the agent entity. Per default it is collidable.  
      AgentEntity entity = new AgentEntity();
      if (collisionType != null) entity.CollisionType = collisionType;
      else entity.CollisionType = SpatialAPI.Entities.Movement.CollisionType.MassiveAgent;
      entity.AgentGuid = ID;  // Insert ID of base agent.
      Entity = entity;        // Set entity object to generic ISpatialEntity reference.

      // Set agent shape. If the agent has no shape yet, create a cube facing north and add at a random position. 
      if (shape != null) Entity.Shape = shape;
      else Entity.Shape = new Cuboid(new Vector3(1.0, 1.0, 1.0), new Vector3(), new Direction());

      // Place the agent in the environment.
      bool success;
      if (shape != null && minPos.IsNull() && maxPos.IsNull()) {
        success = env.Add(Entity, Entity.Shape.Position, Entity.Shape.Rotation);
      }
      else {
        // Random position shall be used. 
        if (minPos.IsNull()) minPos = Vector3.Zero;
        if (maxPos.IsNull()) maxPos = env.MaxDimension;
        success = env.AddWithRandomPosition(Entity, minPos, maxPos, env.IsGrid);
      }

      if (!success) throw new Exception("[SpatialAgent] Agent placement in environment failed (ESC returned 'false')!");
      _env = env; // Save environment reference.       
    }




    /// <summary>
    ///   This function unbinds the agent from the environment.
    ///   It is triggered by the base agent, when alive flag is 'false'.
    /// </summary>
    protected override void Remove() {
      base.Remove();
      _env.Remove(Entity);
    }


    //_________________________________________________________________________
    // IEnvironment related methods and internal spatial object implementation.

    /// <summary>
    ///   The information type of this agent (e.g. its classname).
    /// </summary>
    public Enum InformationType { get; protected set; }


    /// <summary>
    ///   Agent-internal ISpatialEntity implementation.
    /// </summary>
    internal class AgentEntity : ISpatialEntity {

      /// <summary>
      ///   A geometric shape describing this agent's body.
      /// </summary>
      public IShape Shape { get; set; }


      /// <summary>
      ///   This agent's collision type.
      /// </summary>
      public Enum CollisionType { get; set; }


      /// <summary>
      ///   The ID of the base agent.
      /// </summary>
      public Guid AgentGuid { get; set; }
    }


    //_________________________________________________________________________
    // GET methods. The direction is probably not used.

    /// <summary>
    ///   Returns the position of the agent.
    /// </summary>
    /// <returns>A position vector.</returns>
    public Vector3 GetPosition() {
      return Entity.Shape.Position;
    }


    /// <summary>
    ///   Returns the dimension of this agent's bounding box.
    /// </summary>
    /// <returns>A dimension vector.</returns>
    public Vector3 GetDimension() {
      return Entity.Shape.Bounds.Dimension;
    }


    /// <summary>
    ///   Returns the agent's heading.
    /// </summary>
    /// <returns>A direction vector.</returns>
    public Direction GetDirection() {
      return Entity.Shape.Rotation;
    }
  }
}
