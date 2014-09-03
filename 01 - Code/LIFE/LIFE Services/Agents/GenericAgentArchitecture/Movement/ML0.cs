using System;
using ESCTestLayer.Entities;
using ESCTestLayer.Interface;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   This abstract class serves as a base for agent movement.
  ///   It interfaces the Environment Service Component (ESC).
  /// </summary>
  public abstract class ML0 {
    
    private readonly IESC _esc;     // Environment Service Component interface for collision detection.
    private readonly int _agentId;  // Agent identifier, needed for ESC registration.

    public Vector Position   { get; private set;   }   // The agent's center (current position). 
    public Vector TargetPos  { get; protected set; }   // Target position. May be set or auto-calculated.
    public Vector Dimension  { get; set;           }   // Agent's physical dimensions.
    public float  Pitch      { get; private set;   }   // Direction (lateral axis).
    public float  Yaw        { get; private set;   }   // Direction (vertical axis).


    /// <summary>
    ///   Instantiate a new base L0 class. Only available for specializations.
    /// </summary>
    /// <param name="esc">IESC implemenation reference.</param>
    /// <param name="agentId">The ID of the linked agent.</param>
    /// <param name="dim">Agent's physical dimension.</param>
    protected ML0 (IESC esc, int agentId, Vector dim) {
      _esc = esc;
      _agentId = agentId;
      Dimension = dim;

      // Initialization with zeros.
      Position = new Vector(0.0f, 0.0f, 0.0f);
      TargetPos = new Vector(0.0f, 0.0f, 0.0f);
      Pitch = 0.0f;
      Yaw = 0.0f;

      esc.Add (_agentId, GetVector3F(dim));
    }


    /// <summary>
    ///   When the position module is destroyed, the agent is no longer physically present. 
    ///   Remove it from ESC!
    /// </summary>
    ~ML0 () {
      if (_esc != null) _esc.Remove (_agentId);
    }


    /// <summary>
    ///   Set the agent's pitch value [-90° ≤ pitch ≤ 90°].
    /// </summary>
    /// <param name="pitch">New pitch value.</param>
    protected void SetPitch (float pitch) {
      if (pitch >  90) pitch =  90;
      if (pitch < -90) pitch = -90;
      Pitch = pitch;
    }


    /// <summary>
    ///   Set the agent's orientation (compass heading, [0° ≤ yaw lt. 360°].
    /// </summary>
    /// <param name="yaw">New heading.</param>
    protected void SetYaw (float yaw) {
      yaw %= 360;
      if (yaw < 0) yaw += 360;
      Yaw = yaw;
    }


    /// <summary>
    ///   [L0] Perform the movement action. Sends updated values to ESC and receives success or failure.
    /// </summary>
    public void Move() {

      // ESC needs direction vector. So it shall get it. But not normalized ...
      var pitchRad = Pitch * 0.0174532925f;  // Deg -> Rad.
      var yawRad   = Yaw   * 0.0174532925f;      
      var dv = new Vector((float) (Math.Cos(pitchRad) * Math.Cos(yawRad)),
                          (float) (Math.Cos(pitchRad) * Math.Sin(yawRad)),
                          (float) (Math.Sin(pitchRad)));      

      _esc.SetPosition(_agentId, GetVector3F(TargetPos), GetVector3F(dv));
      //TODO Check result for success / failure and behave accordingly.
      //TODO Aktualisierung der Ausgangsposition mit Rückgabe. Vorerst direkte Wertübernahme.
      Position = TargetPos;
    }


    /// <summary>
    ///   Wrapper class that converts the internal vector representation to the ESC class.
    /// </summary>
    /// <param name="vector">The input vector.</param>
    /// <returns>Vector3f class used in ESC.</returns>
    private static Vector3f GetVector3F (Vector vector) {
      return new Vector3f(vector.X, vector.Y, vector.Z);
    }
  }
}
