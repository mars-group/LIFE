using System;
using GenericAgentArchitecture.Agents;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {
  
  /// <summary>
  ///   L1 class: Specializes basic module with speeds and position calculation.
  /// </summary>
  public class ContinuousMover : AgentMover {

    public float Speed   { get; private set; }  // Current movement speed.
    public float PitchAS { get; private set; }  // Pitch changing angular speed.
    public float YawAS   { get; private set; }  // Rotary speed (vertical axis).   


    /// <summary>
    ///   Create an agent mover for continuous environments.
    /// </summary>
    /// <param name="env">Environment interaction interface.</param>
    /// <param name="agent">Agent reference, needed for movement execution.</param>
    /// <param name="data">Container with spatial base data.</param>
    public ContinuousMover(IEnvironment env, SpatialAgent agent, MData data) : base(env, agent, data) {}


    /// <summary>
    ///   Moves the agent. This version uses turning speeds.
    /// </summary>
    /// <param name="speed">The movement speed.</param>
    /// <param name="pitchAS">Pitch changing angular speed.</param>
    /// <param name="yawAS">Rotary speed (vertical axis).</param>
    public void Move(float speed, float pitchAS, float yawAS) {
      PitchAS = pitchAS;
      YawAS = yawAS;
      
      // Calculate pitch and yaw, then call other Move() function.
      var dir = new Direction();
      dir.SetPitch(Data.Direction.Pitch + TickLength*PitchAS);
      dir.SetYaw(Data.Direction.Yaw + TickLength*YawAS);
      Move(speed, dir);
    }


    /// <summary>
    ///   Moves the agent with some speed in a given direction. 
    /// </summary>
    /// <param name="speed">Movement speed.</param>
    /// <param name="dir">The direction.</param>
    public void Move(float speed, Direction dir) {
      Speed = speed;
      TargetDir = dir;
      
      // Calculate target position based on current position, heading and speed.     
      var pitchRad = Direction.DegToRad(TargetDir.Pitch);
      var yawRad = Direction.DegToRad(TargetDir.Yaw);
      var factor = Speed * TickLength;
      TargetPos = new Vector(Data.Position.X, Data.Position.Y, Data.Position.Z);    
      TargetPos.X += (float) (factor * Math.Cos(pitchRad) * Math.Cos(yawRad));
      TargetPos.Y += (float) (factor * Math.Cos(pitchRad) * Math.Sin(yawRad));
      TargetPos.Z += (float) (factor * Math.Sin(pitchRad));

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
    public ContinuousMovementAction MoveTowardsPosition (Vector targetPos, float speed) {

      // Check, if we are already there. Otherwise no need to move anyway.
      var distance = Data.Position.GetDistance(targetPos);
      if (Math.Abs(distance) <= float.Epsilon) 
        return new ContinuousMovementAction(this, 0f, Data.Direction);

      // Get the right direction.
      var dir = CalculateDirectionToTarget(targetPos);

      // Check the speed. If we would go too far, reduce it accordingly.
      if (distance < (speed*TickLength)) speed = distance/TickLength;
  
      // Save calculated values to new movement class and return.
      return new ContinuousMovementAction(this, speed, dir);
    }
  }
}
