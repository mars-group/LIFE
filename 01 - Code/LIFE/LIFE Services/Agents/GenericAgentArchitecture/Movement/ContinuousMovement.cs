using System;
using CommonTypes.DataTypes;
using ESCTestLayer.Interface;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   This class allows to set movement and turning speeds to control the agent.
  /// </summary>
  public class ContinuousMovement : ML1 {


    /// <summary>
    ///   Create a class for movement in a continuous environment.
    /// </summary>
    /// <param name="esc">IESC implemenation reference.</param>
    /// <param name="agentId">The ID of the linked agent.</param>
    /// <param name="dim">Agent's physical dimension.</param>
    public ContinuousMovement(IESC esc, int agentId, Vector3f dim) : base(esc, agentId, dim) { }


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
    public void MoveToPosition (Vector3f targetPos, float speed) {

      // Check, if we are already there. Otherwise no need to move anyway.
      var distance = Position.GetDistance(targetPos);
      if (Math.Abs(distance) <= float.Epsilon) return;

      // Pitch calculation.
      var pitch = (float) Math.Asin((targetPos.Z-Position.Z) / distance) * 57.295779f;


      // Yaw calculation. Radians to degree conversion: 180/π = 57.295
      var yaw = Yaw;
      var distX = targetPos.X - Position.X;
      var distY = targetPos.Y - Position.Y;

      // Check 90° and 270° (arctan infinite) first.      
      if (Math.Abs(distX) <= float.Epsilon) {
        if      (distY > 0f) yaw =  90f;
        else if (distY < 0f) yaw = 270f;
      }

      // Arctan argument fine? Calculate heading then.
      else {
        yaw = (float) Math.Atan(distY / distX) * 57.295779f;
        if (distX < 0f) yaw += 180f;  // Range  90° to 270° correction. 
        if (yaw   < 0f) yaw += 360f;  // Range 270° to 360° correction.        
      }


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
