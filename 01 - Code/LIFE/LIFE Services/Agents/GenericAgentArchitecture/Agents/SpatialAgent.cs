using System;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement.Movers;
using GenericAgentArchitectureCommon.Datatypes;
using SpatialCommon.Datatypes;
using SpatialCommon.Enums;

namespace DalskiAgent.Agents {

  /// <summary>
  ///   This agent is part of a spatial world. It has a position and 
  ///   is registered at an environment to provide collision detection. 
  ///   Per default it is immobile but it can be equipped with a movement module.
  /// </summary>
  public abstract class SpatialAgent : Agent, ISpatialObject {

    private readonly IEnvironment _env;   // IESC implementation for collision detection.
    protected readonly DataAccessor Data; // R/O container for spatial data access.
    protected AgentMover Mover;           // Class for agent movement. 


    /// <summary>
    ///   Instantiate a new agent with spatial data. Only available for specializations.
    /// </summary>
    /// <param name="exec">Agent execution container reference.</param>
    /// <param name="env">Environment implementation reference.</param>
    /// <param name="collision">Collision type defines with whom the agent may collide.</param>
    /// <param name="pos">The initial position. If null, it is tried to be set randomly.</param>
    /// <param name="dim">Dimension of the agent. If null, then (1,1,1).</param>
    /// <param name="dir">Direction of the agent. If null, then 0°.</param>
    protected SpatialAgent(IExecution exec, IEnvironment env, CollisionType collision, Vector pos, Vector dim = null, Direction dir = null): base(exec) {
      _env = env;
      _env.AddObject(this, collision, pos, out Data, dim, dir);  // Enlist the agent in environment.
    }

    
    /// <summary>
    ///   Instantiate a new agent with spatial data. Only available for specializations.
    /// </summary>
    /// <param name="exec">Agent execution container reference.</param>
    /// <param name="env">Environment implementation reference.</param>
    /// <param name="collision">Collision type defines with whom the agent may collide.</param>
    /// <param name="minPos">Minimum coordinate for random position.</param>
    /// <param name="maxPos">Maximum coordinate for random position.</param>
    /// <param name="dim">Dimension of the agent. If null, then (1,1,1).</param>
    /// <param name="dir">Direction of the agent. If null, then 0°.</param>
    protected SpatialAgent(IExecution exec, IEnvironment env, CollisionType collision, Vector minPos, Vector maxPos, Vector dim = null, Direction dir = null) : base(exec) {
      if (env is ESCRectAdapter) {
        _env = env;
        ((ESCRectAdapter)_env).AddObject(this, collision, minPos, maxPos, out Data, dim, dir);  // Enlist the agent in environment.        
      }
      else throw new Exception("[SpatialAgent] Constructor error: Environment does not support interval placement. Use ESCAdapter instead!");
    }


    /// <summary>
    ///   Initialization function (post-constructor). In this case, 
    ///   it just calls the base init method for execution registration.
    /// </summary>
    protected new void Init() {
      base.Init();
    }


    /// <summary>
    ///   This function unbinds the agent from the environment.
    ///   It is triggered by the base agent, when alive flag on 'false'.
    /// </summary>
    protected override void Remove() {
      base.Remove();
      _env.RemoveObject(this);
    }


    //-------------------------------------------------------------------------
    // GET methods. The direction is probably not used.

    /// <summary>
    ///   Returns the position of the agent.
    /// </summary>
    /// <returns>A position vector.</returns>
    public Vector GetPosition() {
      return Data.Position;
    }


    /// <summary>
    ///   Returns the agent's heading.
    /// </summary>
    /// <returns>A direction vector.</returns>
    public Direction GetDirection() {
      return Data.Direction;
    }


    /// <summary>
    ///   Returns the agent's dimension.
    /// </summary>
    /// <returns>The dimension (as bounding box).</returns>
    public Vector GetDimension() {
      return Data.Dimension;
    }
  }
}
