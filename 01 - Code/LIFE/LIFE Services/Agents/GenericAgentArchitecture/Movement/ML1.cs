using System;
using CommonTypes.DataTypes;
using ESCTestLayer.Interface;

namespace GenericAgentArchitecture.Movement {
  
  /// <summary>
  ///   This class extends the basic positioning system with movement and turning speeds.
  /// </summary>
  public abstract class ML1 : ML0 {

    public float Speed      { get; protected set; } // Current movement speed.
    public float PitchAS    { get; protected set; } // Pitch changing angular speed.
    public float YawAS      { get; protected set; } // Rotary speed (vertical axis).   
    public float TickLength { get; set;           } // Timelength of a simulation tick. 


    /// <summary>
    ///   L1 class, specializes basic module with speeds and position calculation.
    /// </summary>
    /// <param name="esc">IESC implemenation reference.</param>
    /// <param name="agentId">The ID of the linked agent.</param>
    /// <param name="dim">Agent's physical dimension.</param>
    protected ML1(IESC esc, int agentId, Vector dim) : base(esc, agentId, dim) {
      Speed = 0;
      PitchAS = 0;
      YawAS = 0;
      TickLength = 1; // Tick length is per default one unit (of whatever). 
    }


    /// <summary>
    ///   [L1] Calculate direction and target position, continue with L0. 
    /// </summary>
    public new void Move() {

      // Calculate pitch and yaw (in case AS'es are not used, nothing happens here).
      SetPitch(Pitch + PitchAS*TickLength);
      SetYaw(Yaw + YawAS*TickLength);

      // Determine target position based on calculated values.
      var pitchRad = Pitch*0.0174532925f; // Deg -> Rad.
      var yawRad = Yaw*0.0174532925f;
      var factor = Speed*TickLength;
      var targetPos = new Vector(Position.X, Position.Y, Position.Z);
      targetPos.X += (float) (factor * Math.Cos(pitchRad) * Math.Cos(yawRad));
      targetPos.Y += (float) (factor * Math.Cos(pitchRad) * Math.Sin(yawRad));
      targetPos.Z += (float) (factor * Math.Sin(pitchRad));
      TargetPos = targetPos;

      // Execute L0 call. 
      base.Move();
    }
  }
}