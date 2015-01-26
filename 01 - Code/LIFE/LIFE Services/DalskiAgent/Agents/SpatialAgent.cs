using System;
using DalskiAgent.Movement.Movers;
using LifeAPI.Environment;
using LifeAPI.Layer;
using LifeAPI.Spatial;
using SpatialCommon.Shape;
using SpatialCommon.Transformation;


namespace DalskiAgent.Agents {

  /// <summary>
  ///   This agent is part of a spatial world. It has a position and 
  ///   is registered at an environment to provide collision detection. 
  ///   Per default it is immobile but it can be equipped with a movement module.
  /// </summary>
  public abstract class SpatialAgent : Agent, ISpatialEntity {

    public IShape Shape { get; set; }      // Shape describing this agent's body.
    private readonly IEnvironment _env;     // IESC implementation for collision detection.
    protected AgentMover Mover;             // Class for agent movement. 
    protected CollisionType _collisionType;  // The agent's collision type.      


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
                           Vector3 pos = default(Vector3), Direction dir = null) : 
      
      // Create the base agent. Per default it is collidable.
      base(layer, regFkt, unregFkt) {
      _collisionType = LifeAPI.Spatial.CollisionType.MassiveAgent;
      
      // Check, if the agent already has a direction and a form. If not, create a cube facing north.
      if (dir == null) dir = new Direction();
      if (shape == null) shape = new Cuboid(new Vector3(1.0, 1.0, 1.0), pos, dir);
      Shape = shape;

      // Place the agent in the environment.
      bool success;
      if (!pos.IsNull()) success = env.Add(this, pos, dir);
      else success = env.AddWithRandomPosition(this, Vector3.Zero, env.MaxDimension, env.IsGrid);    
      if (!success) throw new Exception("[SpatialAgent] Agent placement in environment failed (ESC returned 'false')!");
      
      // Save references for later use.    
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
    ///   Returns the agent's heading.
    /// </summary>
    /// <returns>A direction vector.</returns>
    public Direction GetDirection() {
        return Shape.Rotation;
    }


      /// <summary>
      ///   Returns this agent's collision type.
      /// </summary>
      /// <returns>A collision type enum (default: 'MassiveAgent').</returns>
      public Enum CollisionType { get { return _collisionType; } }



//    //TODO| Here lies the error. The shape has to be reset by the ESC, because it holds 
//    //TODO| the position data. So we don't get the random placement result => all (0,0,0)
//    public IShapeOld Shape {
//      get {
//        return new ExploreRectShape(MyRectFactory.Rectangle(GetPosition(), new Vector3(1,1,1)));
//      }
//      set {
//        _shape = new Quad(new Vector3(1,1,1), value.GetPosition(), new Direction());
//      }
//    }


      public Enum InformationType {
          get { return CollisionType; //TODO ...
          }
      }
  }
}
