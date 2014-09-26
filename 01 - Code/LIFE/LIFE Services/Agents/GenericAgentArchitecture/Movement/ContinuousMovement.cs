using System;
using GenericAgentArchitectureCommon.Interfaces;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   This class allows to set movement and turning speeds to control the agent.
  /// </summary>
  public class ContinuousMovement : AbstractMovement, IInteraction {


    /// <summary>
    ///   Create a movement action for continuous environments.
    /// </summary>
    /// <param name="speed">The new movement speed.</param>
    /// <param name="pitch">New pitch [or vertical turning speed].</param>
    /// <param name="yaw">New yaw [or horizontal turning speed].</param>
    /// <param name="angSpd">If set, the pitch/yaw values are speeds (def.: false).</param>
    public ContinuousMovement(float speed, float pitch, float yaw, bool angSpd = false) {     
      if (angSpd) { // Calculate pitch and yaw (in case AS'es are not used, nothing happens here).
        pitch = Data.Pitch + pitch*TickLength;
        yaw = Data.Yaw + yaw*TickLength;
      }      
      Data.SetPitch(pitch);
      Data.SetYaw(yaw);
      Data.Speed = speed;
    }



    public void Execute() {

      // Determine target position based on calculated values.
      var pitchRad = Data.Pitch*MovementServices.Deg2Rad;
      var yawRad = Data.Yaw*MovementServices.Deg2Rad;
      var factor = Data.Speed*TickLength;
      var targetPos = new Vector(Data.Position.X, Data.Position.Y, Data.Position.Z);
      targetPos.X += (float) (factor * Math.Cos(pitchRad) * Math.Cos(yawRad));
      targetPos.Y += (float) (factor * Math.Cos(pitchRad) * Math.Sin(yawRad));
      targetPos.Z += (float) (factor * Math.Sin(pitchRad));
      TargetPos = targetPos;

      // Execute L0 call. 
      Move();
    }
  }
}
