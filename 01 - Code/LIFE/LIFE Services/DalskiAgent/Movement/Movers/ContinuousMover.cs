using System;
using DalskiAgent.Agents;
using DalskiAgent.Movement.Actions;
using LifeAPI.Environment;
using LifeAPI.Spatial;
using SpatialCommon.Transformation;

namespace DalskiAgent.Movement.Movers {
  
  /// <summary>
  ///   L1 class: Specializes basic module with speeds and position calculation.
  /// </summary>
  public class ContinuousMover : AgentMover {

    public double Speed  { get; private set; }  // Current movement speed.
    public float PitchAs { get; private set; }  // Pitch changing angular speed.
    public float YawAs   { get; private set; }  // Rotary speed (vertical axis).   


    /// <summary>
    ///   Create an agent mover for continuous environments.
    /// </summary>
    /// <param name="env">Environment interaction interface.</param>
    /// <param name="agent">Agent reference, needed for movement execution.</param>
    public ContinuousMover(IEnvironment env, SpatialAgent agent) : base(env, agent) {}


    /// <summary>
    ///   Moves the agent. This version uses turning speeds.
    /// </summary>
    /// <param name="speed">The movement speed.</param>
    /// <param name="pitchAs">Pitch changing angular speed.</param>
    /// <param name="yawAs">Rotary speed (vertical axis).</param>
    public void Move(double speed, float pitchAs, float yawAs) {
      PitchAs = pitchAs;
      YawAs = yawAs;
      
      // Calculate pitch and yaw, then call lower Move() function.
      var dir = new Direction();
      dir.SetPitch(Agent.GetDirection().Pitch + TickLength*PitchAs);
      dir.SetYaw(Agent.GetDirection().Yaw + TickLength*YawAs);
      Move(speed, dir);
    }


    /// <summary>
    ///   Moves the agent with some speed in a given direction. 
    /// </summary>
    /// <param name="speed">Movement speed.</param>
    /// <param name="dir">The direction.</param>
    public void Move(double speed, Direction dir) {
      Speed = speed;
      TargetDir = dir;
      
      // Calculate target position based on current position, heading and speed.     
      var pitchRad = Direction.DegToRad(TargetDir.Pitch);
      var yawRad = Direction.DegToRad(TargetDir.Yaw);
      var factor = Speed * TickLength;
      var x = factor * Math.Cos(pitchRad) * Math.Sin(yawRad);
      var y = factor * Math.Cos(pitchRad) * Math.Cos(yawRad);
      var z = factor * Math.Sin(pitchRad);    
      MovementVector = new Vector3(x, y, z);

      // Execute L0 call. 
      Move();
    }


    /// <summary>
    ///   This function automatically sets the reference, speed and yaw & pitch 
    ///   values to go to the supplied point. It then returns an appropriate action. 
    /// </summary>
    /// <param name="targetPos">A point the agent shall go to.</param>
    /// <param name="speed">The agent's movement speed.</param>
    /// <returns>A movement action, ready for execution. </returns>
    public ContinuousMovementAction MoveTowardsPosition (Vector3 targetPos, double speed) {

      // Check, if we are already there. Otherwise no need to move anyway.
      var distance = Agent.GetPosition().GetDistance(targetPos);
      if (Math.Abs(distance) <= float.Epsilon) 
        return new ContinuousMovementAction(this, 0f, Agent.GetDirection());

      // Get the right direction.
      var dir = CalculateDirectionToTarget(targetPos);

      // Check the speed. If we would go too far, reduce it accordingly.
      if (distance < (speed*TickLength)) speed = distance/TickLength;
  
      // Save calculated values to new movement class and return.
      return new ContinuousMovementAction(this, speed, dir);
    }
  }
}
