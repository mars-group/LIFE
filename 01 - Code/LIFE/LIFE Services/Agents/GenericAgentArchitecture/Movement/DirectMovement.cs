using ESCTestLayer.Interface;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {
  
  /// <summary>
  ///   This class enables basic agent movement.
  /// </summary>
  public class DirectMovement : ML0 {
    
   
    /// <summary>
    ///   Create a class for direct movement.
    /// </summary>
    /// <param name="esc">IESC implemenation reference.</param>
    /// <param name="escInit">Initialization data needed by ESC.</param>
    /// <param name="pos">Agent's initial position.</param>
    /// <param name="dim">Agent's physical dimension.</param>
    public DirectMovement(IESC esc, ESCInitData escInit, TVector pos, TVector dim) : base(esc, escInit, pos, dim) { }


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
    public void SetTargetPosition(TVector target) {
      TargetPos = new Vector(target.X, target.Y, target.Z);
    }
  }
}