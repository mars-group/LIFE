﻿using System;

namespace GenericAgentArchitecture.Dummies {
  
  /// <summary>
  ///   Position management class for an agent. It shall be used by movement interactions.
  /// </summary>
  public class Position {

    private bool _is3D;                          // Dimension flag: false: 2D, true: 3D.
    public Float3 Center  { get; set; }          // Center coordinates of the agent. 
    public float Pitch    { get; private set; }  // Direction (lateral axis).
    public float Yaw      { get; private set; }  // Direction (vertical axis).
    public float Velocity { get; set; }          // Agent's speed.



    /// <summary>
    ///   Create a new position object given the initial values [2D version].
    /// </summary>
    public Position(float x, float y, float yaw, float velocity) {
      Center = new Float3(x, y, 0);
      Pitch = 0;
      Yaw = yaw;
      Velocity = velocity;
      _is3D = false;
    }


    /// <summary>
    ///   Create a new position object given the initial values [3D version].
    /// </summary>
    public Position(float x, float y, float z, float pitch, float yaw, float velocity) {
      Center = new Float3(x, y, z);
      Pitch = pitch;
      Yaw = yaw;
      Velocity = velocity;
      _is3D = true;
    }


    /// <summary>
    ///   Calculate the new agent position, based on current position, directions and speed.
    /// </summary>
    /// <returns>A three-dimensional float structure with the new position.</returns>
    public Float3 CalculateNewPosition() {
      var pitchRad = Pitch * 0.0174532925f;  // Deg -> Rad.
      var yawRad   = Yaw   * 0.0174532925f;  
      var newPos   = new Float3 (Center.X, Center.Y, Center.Z);
      newPos.X += (float) (Velocity*Math.Cos(pitchRad)*Math.Cos(yawRad));
      newPos.Y += (float) (Velocity*Math.Cos(pitchRad)*Math.Sin(yawRad));
      newPos.Z += (float) (Velocity*Math.Sin(pitchRad));
      return newPos;
    }


    /// <summary>
    ///   Set the agent's orientation (compass heading, [0° ≤ yaw lt. 360°].
    /// </summary>
    /// <param name="yaw">New heading.</param>
    public void SetYaw(float yaw) {
      yaw %= 360;
      if (yaw < 0)
        yaw += 360;
      Yaw = yaw;
    }


    /// <summary>
    ///   Set the agent's pitch value [-90° ≤ pitch ≤ 90°].
    /// </summary>
    /// <param name="pitch">New pitch value.</param>
    public void SetPitch(float pitch) {
      if (pitch > 90)
        pitch = 90;
      if (pitch < -90)
        pitch = -90;
      Pitch = pitch;
    }


    /// <summary>
    ///   Return the cartesian distance of two positions. 
    /// </summary>
    /// <param name="pos">The position to measure the distance to.</param>
    /// <returns>The distance value.</returns>
    public float GetDistance(Position pos) {
      return Center.GetDistance(pos.Center);
    }


    /// <summary>
    ///   Output the position.
    /// </summary>
    /// <returns>String with component-based notation.</returns>
    public override string ToString() {
      return !_is3D ? String.Format("({0,2}|{1,2})",       Center.X,Center.Y)
                    : String.Format("({0,2}|{1,2}|{2,2})", Center.X,Center.Y,Center.Z);
    }
  }


  /// <summary>
  ///   This structure holds x, y, and z values of a position.
  /// </summary>
  public struct Float3 {
    public float X, Y, Z;

    public Float3(float x, float y) {
      X = x;
      Y = y;
      Z = 0;
    }

    public Float3(float x, float y, float z) {
      X = x;
      Y = y;
      Z = z;
    }

    /// <summary>
    /// Calculate point-to-point distance.
    /// </summary>
    /// <param name="pos">The target point.</param>
    /// <returns>Euclidian distance value.</returns>
    public float GetDistance(Float3 pos) {
      return (float) Math.Sqrt((X - pos.X)*(X - pos.X) +
                               (Y - pos.Y)*(Y - pos.Y) +
                               (Z - pos.Z)*(Z - pos.Z));      
    }
  };
}