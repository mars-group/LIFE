﻿using System;

namespace GenericAgentArchitectureCommon.Datatypes {
  
  /// <summary>
  ///   Direction class, holds pitch and yaw value and provides some conversions. 
  /// </summary>
  public class Direction {
    
    public double  Pitch { get; private set; }   // Direction (lateral axis).
    public double Yaw { get; private set; }   // Direction (vertical axis).     


    /// <summary>
    ///   Set the agent's pitch value [-90° ≤ pitch ≤ 90°].
    /// </summary>
    /// <param name="pitch">New pitch value.</param>
    public void SetPitch(double pitch) {
      if (pitch >  90) pitch =  90;
      if (pitch < -90) pitch = -90;
      Pitch = pitch;
    }


    /// <summary>
    ///   Set the agent's orientation (compass heading, [0° ≤ yaw lt. 360°].
    /// </summary>
    /// <param name="yaw">New heading.</param>
    public void SetYaw(double yaw) {
      yaw %= 360;
      if (yaw < 0) yaw += 360;
      Yaw = yaw;
    }


    /// <summary>
    ///   Create a directional vector based on pitch and yaw values.
    /// </summary>
    /// <returns>A directional vector as structure.</returns>
    public Vector GetDirectionalVector() {     
      var pitchRad = DegToRad(Pitch);
      var yawRad   = DegToRad(Yaw);      
      return new Vector(Math.Cos(pitchRad) * Math.Sin(yawRad),
                        Math.Cos(pitchRad) * Math.Cos(yawRad),
                        Math.Sin(pitchRad)).GetNormalVector();        
    }


    /// <summary>
    ///   Use a directional vector to create pitch and yaw values.
    /// </summary>
    /// <param name="vector">The vector to set.</param>
    public void SetDirectionalVector(Vector vector) {
      SetPitch(RadToDeg(Math.Asin(vector.Z / vector.GetLength())));
      var yaw = Yaw;

      // Check 90° and 270° (arctan infinite) first.      
      if (Math.Abs(vector.Y) <= double.Epsilon) {
        if      (vector.X > 0f) yaw = 90f;
        else if (vector.X < 0f) yaw = 270f;
      }

      // Arctan argument fine? Calculate heading then.    
      else {
        yaw = RadToDeg(Math.Atan(vector.X / vector.Y));
        if (vector.Y < 0f) yaw += 180f;  // Range  90° to 270° correction. 
        if (yaw      < 0f) yaw += 360f;  // Range 270° to 360° correction.        
      }
      SetYaw(yaw);
    }


    /// <summary>
    ///   Degree → radians conversion (π/180).
    /// </summary>
    /// <param name="angle">Angle in degree.</param>
    /// <returns>Angle in radians.</returns>
    public static double DegToRad(double angle) {
      return angle*0.0174532925f;
    }


    /// <summary>
    ///   Radians → degree conversion (180/π).
    /// </summary>
    /// <param name="angle">Angle in radians.</param>
    /// <returns>Angle in degree.</returns>
    public static double RadToDeg(double angle) {
      return angle*57.295779f;
    }
  }
}
