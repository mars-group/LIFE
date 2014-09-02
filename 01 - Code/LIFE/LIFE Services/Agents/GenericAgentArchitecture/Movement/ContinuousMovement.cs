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
    public ContinuousMovement(IESC esc, int agentId, Vector dim) : base(esc, agentId, dim) {}


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
