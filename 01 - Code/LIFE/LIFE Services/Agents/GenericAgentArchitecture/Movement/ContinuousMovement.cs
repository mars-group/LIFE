using System;
using ESCTestLayer.Interface;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   This class allows to set movement and turning speeds to control the agent.
  /// </summary>
  public class ContinuousMovement : ML1 {


    /// <summary>
    ///   Create a class for movement in a continuous environment.
    /// </summary>
    /// <param name="esc">IESC implemenation reference.</param>
    /// <param name="escInit">Initialization data needed by ESC.</param>
    /// <param name="pos">Agent's initial position.</param> 
    /// <param name="dim">Agent's physical dimension.</param>
    public ContinuousMovement(IESC esc, ESCInitData escInit, TVector pos, TVector dim) : base(esc, escInit, pos, dim) { }


    /// <summary>
    ///   Set a new movement speed.
    /// </summary>
    /// <param name="speed">The new movement speed.</param>
    public void SetMovementSpeed(float speed) {
      Speed = speed;
    }


    /// <summary>
    ///   Set a new vertical turning speed.
    /// </summary>
    /// <param name="pitchSpeed">The new lateral turning speed.</param>
    public void SetPitchSpeed(float pitchSpeed) {
      PitchAS = pitchSpeed;
    }


    /// <summary>
    ///   Set a new horizontal turning speed.
    /// </summary>
    /// <param name="yawSpeed">The new rotary speed.</param>
    public void SetYawSpeed(float yawSpeed) {
      YawAS = yawSpeed;
    }


    /// <summary>
    ///   This function automatically sets the yaw and pitch values to go to 
    ///   the supplied point. It then executes movement with the given speed. 
    /// </summary>
    /// <param name="targetPos">A point the agent shall go to.</param>
    /// <param name="speed">The agent's movement speed.</param>
    public void MoveToPosition (TVector targetPos, float speed) {

      // Check, if we are already there. Otherwise no need to move anyway.
      var distance = VectorToStruct(Position).GetDistance(targetPos);
      if (Math.Abs(distance) <= float.Epsilon) return;

      // Pitch and yaw calculation.
      var pitch = (float) Math.Asin((targetPos.Z-Position.Z) / distance) * 57.295779f;
      var yaw = CalculateYawToTarget(targetPos);

      // Check the speed. If we would go too far, reduce it accordingly.
      if (distance < (speed*TickLength)) speed = distance/TickLength;
  
      // Save calculated values to base class and set movement speed.
      base.SetPitch(pitch);
      base.SetYaw(yaw);
      SetMovementSpeed(speed);

      // Disable turning speeds and execute movement.
      PitchAS = 0f;
      YawAS = 0f;
      Move();
    }



    // These functions are added for convenience. Actually, they do not belong here ...
    #region 

    /// <summary>
    ///   Set the agent's pitch value [-90° ≤ pitch ≤ 90°].
    /// </summary>
    /// <param name="pitch">New pitch value.</param>
    public new void SetPitch(float pitch) {
      base.SetPitch(pitch);
    }

    /// <summary>
    ///   Set the agent's orientation (compass heading, [0° ≤ yaw lt. 360°].
    /// </summary>
    /// <param name="yaw">New heading.</param>
    public new void SetYaw(float yaw) {
      base.SetYaw(yaw);
    }

    #endregion
  }
}
