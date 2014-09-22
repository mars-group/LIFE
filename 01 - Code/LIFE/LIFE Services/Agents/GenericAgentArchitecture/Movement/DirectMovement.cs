using CommonTypes.DataTypes;
using ESCTestLayer.Interface;

namespace GenericAgentArchitecture.Movement {
  
  /// <summary>
  ///   This class enables basic agent movement.
  /// </summary>
  public class DirectMovement : ML0 {
    
   
    /// <summary>
    ///   Create a class for direct movement.
    /// </summary>
    /// <param name="esc">IESC implemenation reference.</param>
    /// <param name="agentId">The ID of the linked agent.</param>
    /// <param name="dim">Agent's physical dimension.</param>
    public DirectMovement(IESC esc, int agentId, Vector dim) : base(esc, agentId, dim) { }


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

    /// <summary>
    ///   Set a new target position.
    /// </summary>
    /// <param name="target">The position the agent shall try to gain.</param>
    public void SetTargetPosition(Vector target) {
      TargetPos = target;
    }
  }
}