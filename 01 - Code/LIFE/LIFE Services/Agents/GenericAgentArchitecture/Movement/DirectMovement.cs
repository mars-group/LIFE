using GenericAgentArchitectureCommon.Interfaces;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {
  
  /// <summary>
  ///   This class enables basic agent movement.
  /// </summary>
  public class DirectMovement : AbstractMovement, IInteraction {


    /// <summary>
    ///   Create a direct movement action.
    /// </summary>
    /// <param name="position">The new target position.</param>
    /// <param name="pitch">New pitch value.</param>
    /// <param name="yaw">New heading.</param>
    public DirectMovement(TVector position, float pitch, float yaw) {
      TargetPos = new Vector(position.X, position.Y, position.Z);
      Data.SetPitch(pitch);
      Data.SetYaw(yaw);
    }


    public void Execute() {
      Move();
    }
  }
}