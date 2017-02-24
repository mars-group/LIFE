﻿using LIFE.Components.Agents.AgentTwo.Environment;
using LIFE.Components.Agents.AgentTwo.Perception;
using LIFE.Components.Agents.AgentTwo.Reasoning;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC. SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.Agents.AgentTwo.Movement {

  /// <summary>
  ///   Agent mover for three-dimensional cartesian environments.
  /// </summary>
  public class AgentMover3D : AgentMover {

    private readonly IEnvironment _env;      // The environment implementation to use.
    private readonly CartesianPosition _pos; // Agent position class.


    /// <summary>
    ///   Create a new cartesian 3D mover.
    ///   This function is automatically invoked in the abstract agent!
    /// </summary>
    /// <param name="env">The environment to use (probably some crappy ESC).</param>
    /// <param name="agentPos">Agent position data structure.</param>
    /// <param name="sensorArray">The agent's sensor array (to provide movement feedback).</param>
    public AgentMover3D(IEnvironment env, CartesianPosition agentPos, SensorArray sensorArray)
      : base(sensorArray) {
      _env = env;
      _pos = agentPos;
    }


    /// <summary>
    ///   Try to insert this agent into the environment at the given position.
    /// </summary>
    /// <param name="x">Agent start position (x-coordinate).</param>
    /// <param name="y">Agent start position (y-coordinate).</param>
    /// <param name="z">Agent start position (z-coordinate).</param>
    /// <returns>Success flag. If failed, the agent may not be moved!</returns>
    public bool InsertIntoEnvironment(double x, double y, double z) {
      _pos.Shape = new Cuboid(new Vector3(1,1,1), new Vector3(x,y,z));
      return _env.Add(_pos, _pos.Shape.Position);
    }


    /// <summary>
    ///   Move the agent forward with a given speed.
    /// </summary>
    /// <param name="distance">The distance to move.</param>
    /// <returns>Interaction expressing this movement.</returns>
    public MovementAction MoveForward(double distance) {
      return MoveInDirection(distance, _pos.Yaw, _pos.Pitch);
    }


    /// <summary>
    ///   Move the agent into a direction.
    /// </summary>
    /// <param name="distance">The distance to move.</param>
    /// <param name="yaw">New agent heading.</param>
    /// <param name="pitch">Agent climb angle.</param>
    /// <returns>An interaction describing the movement.</returns>
    public MovementAction MoveInDirection(double distance, double yaw, double pitch) {
      var dir = new Direction();
      dir.SetYaw(yaw);
      dir.SetPitch(pitch);
      var vec =  dir.GetDirectionalVector() * distance;
      return new MovementAction(() => {
        var result = _env.Move(_pos, vec, dir);
        MovementSensor.SetMovementResult(result);
      });
    }


    /// <summary>
    ///   Move the agent to a position.
    /// </summary>
    /// <param name="distance">The distance to move.</param>
    /// <param name="x">X-coordinate target position.</param>
    /// <param name="y">Y-coordinate of target position.</param>
    /// <param name="z">Z-coordinate of target position.</param>
    /// <returns>An interaction describing the movement.</returns>
    public MovementAction MoveTowardsPosition(double distance, double x, double y, double z) {
      var diff = new Vector3(x-_pos.X, y-_pos.Y, z-_pos.Z);
      var dir = new Direction();
      dir.SetDirectionalVector(diff);
      var dist = new Vector3(x, y, z).GetDistance(diff); //| Check the traveling distance.
      if (distance > dist) distance = dist;              //| If we would go too far, reduce it accordingly.
      return MoveInDirection(distance, dir.Yaw, dir.Pitch);
    }


    /// <summary>
    ///   Set the agent to a new position.
    /// </summary>
    /// <param name="x">X-coordinate target position.</param>
    /// <param name="y">Y-coordinate of target position.</param>
    /// <param name="z">Z-coordinate of target position.</param>
    /// <returns>An interaction describing the movement.</returns>
    public MovementAction SetToPosition(double x, double y, double z) {
      return MoveTowardsPosition(double.MaxValue, x, y, z);
    }
  }
}