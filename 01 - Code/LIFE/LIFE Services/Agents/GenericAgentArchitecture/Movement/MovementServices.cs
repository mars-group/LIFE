using System;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {
  public static class MovementServices {
    
    public const float Deg2Rad = 0.0174532925f;  // π/180 (degree → radians conversion).
    public const float Rad2Deg = 57.295779f;     // 180/π (radians → degree conversion).
    public const float Sqrt2   = 1.4142f;        // The square root of 2.

    public static float TickLength = 1.0f;        // Timelength of a simulation tick.

    /// <summary>
    ///   This function automatically sets the yaw and pitch values to go to 
    ///   the supplied point. It then executes movement with the given speed. 
    /// </summary>
    /// <param name="targetPos">A point the agent shall go to.</param>
    /// <param name="data">Container with current movement data.</param>
    /// <param name="speed">The agent's movement speed.</param>
    public static ContinuousMovement MoveToPosition (TVector targetPos, MData data, float speed) {

      // Check, if we are already there. Otherwise no need to move anyway.
      var tp = new Vector(targetPos.X, targetPos.Y, targetPos.Z);
      var distance = data.Position.GetDistance(tp);
      if (Math.Abs(distance) <= float.Epsilon) 
        return new ContinuousMovement(0.0f, 0.0f, 0.0f, true);

      // Pitch and yaw calculation.
      var pitch = (float) Math.Asin((targetPos.Z-data.Position.Z) / distance) * Rad2Deg;
      var yaw = CalculateYawToTarget(targetPos, data);

      // Check the speed. If we would go too far, reduce it accordingly.
      if (distance < (speed*TickLength)) speed = distance/TickLength;
  
      // Save calculated values to new movement class and return.
      return new ContinuousMovement(speed, pitch, yaw);
    }


    /// <summary>
    ///   Calculate the yaw to a given heading.
    /// </summary>
    /// <param name="target">The target to get orientation to.</param>
    /// <param name="data">Container with current movement data.</param>
    /// <returns>The yaw (corrected to 0 - lt. 360). </returns>
    public static float CalculateYawToTarget(TVector target, MData data) {
      var yaw = data.Yaw;
      var distX = target.X - data.Position.X;
      var distY = target.Y - data.Position.Y;

      // Check 90° and 270° (arctan infinite) first.      
      if (Math.Abs(distX) <= float.Epsilon) {
        if      (distY > 0f) yaw =  90f;
        else if (distY < 0f) yaw = 270f;
      }

      // Arctan argument fine? Calculate heading then.    
      else { // Radians to degree conversion: 180/π = 57.295
        yaw = (float) Math.Atan(distY / distX) * 57.295779f;
        if (distX < 0f) yaw += 180f;  // Range  90° to 270° correction. 
        if (yaw   < 0f) yaw += 360f;  // Range 270° to 360° correction.        
      }
      return yaw;
    }


    /// <summary>
    ///   Create a directional vector based on pitch and yaw values.
    /// </summary>
    /// <param name="data">Position object to read.</param>
    /// <returns>A directional vector as structure.</returns>
    public static TVector GetDirectionalVector(MData data) {     
      var pitchRad = data.Pitch * Deg2Rad;
      var yawRad   = data.Yaw   * Deg2Rad;      
      return new TVector((float) (Math.Cos(pitchRad) * Math.Cos(yawRad)),
                         (float) (Math.Cos(pitchRad) * Math.Sin(yawRad)),
                         (float) (Math.Sin(pitchRad))).Normalize();        
    }
  }
}
