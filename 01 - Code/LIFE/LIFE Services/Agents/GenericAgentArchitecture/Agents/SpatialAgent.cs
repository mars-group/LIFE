﻿using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using DalskiAgent.Movement.Movers;

namespace DalskiAgent.Agents {

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
    /// <param name="exec">Agent execution container reference.</param>
    /// <param name="env">Environment implementation reference.</param>
    /// <param name="pos">The initial position. If null, it is tried to be set randomly.</param>
    protected SpatialAgent(IExecution exec, IEnvironment env, Vector pos) : base(exec) {
      _env = env;
      _env.AddAgent(this, pos, out Data); // Enlist the agent in environment.
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