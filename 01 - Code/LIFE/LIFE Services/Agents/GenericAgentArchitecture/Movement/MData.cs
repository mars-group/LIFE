using System;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {
  
  /// <summary>
  ///   This class holds all spatial data for an agent.
  /// </summary>
  public class MData {
    
    public float  Pitch { get; private set; } // Direction (lateral axis).
    public float  Yaw   { get; private set; } // Direction (vertical axis).
    public float  Speed;                      // Movement speed.
    public Vector Position;                   // The agent's current position.

    /// <summary>
    ///   Create a new spatial data set.
    /// </summary>
    /// <param name="pos">The initial position (common transport vector).</param>
    public MData(TVector pos) {
      Position = new Vector(pos.X, pos.Y, pos.Z);
      Pitch = 0.0f;  // Default facing is straight line northbound. 
      Yaw = 0.0f;    // May be overwritten in specific constructor.
      Speed = 0.0f;
    }


    /// <summary>
    ///   Set the agent's pitch value [-90° ≤ pitch ≤ 90°].
    /// </summary>
    /// <param name="pitch">New pitch value.</param>
    public void SetPitch (float pitch) {
      if (pitch >  90) pitch =  90;
      if (pitch < -90) pitch = -90;
      Pitch = pitch;
    }


    /// <summary>
    ///   Set the agent's orientation (compass heading, [0° ≤ yaw lt. 360°].
    /// </summary>
    /// <param name="yaw">New heading.</param>
    public void SetYaw (float yaw) {
      yaw %= 360;
      if (yaw < 0) yaw += 360;
      Yaw = yaw;
    }
  }
}
