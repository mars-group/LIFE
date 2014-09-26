using System;
using ESCTestLayer.Interface;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   This abstract class serves as a base for agent movement.
  ///   It interfaces the Environment Service Component (ESC).
  /// </summary>
  public abstract class ML0 {
    
    private readonly IESC _esc;     // Environment Service Component interface for collision detection.
    private readonly int _agentId;  // Agent identifier, needed for ESC registration.

    public Vector Position  { get; private set;   }   // The agent's center (current position). 
    public Vector TargetPos { get; protected set; }   // Target position. May be set or auto-calculated.
    public float  Pitch     { get; private set;   }   // Direction (lateral axis).
    public float  Yaw       { get; private set;   }   // Direction (vertical axis).


    /// <summary>
    ///   Instantiate a new base L0 class. Only available for specializations.
    /// </summary>
    /// <param name="esc">IESC implemenation reference.</param>
    /// <param name="escInit">Initialization data needed by ESC.</param>
    /// <param name="pos">Agent's initial position.</param>
    /// <param name="dim">Agent's physical dimension.</param>
    protected ML0 (IESC esc, ESCInitData escInit, TVector pos, TVector dim) {
      _esc = esc;
      _agentId = escInit.AgentId;

      // Initialization with original position, facing northbound.
      Position  = new Vector(pos.X, pos.Y, pos.Z);
      TargetPos = new Vector(pos.X, pos.Y, pos.Z);
      Pitch = 0.0f;
      Yaw = 0.0f;

      // Enlist the agent and place it at its current position. 
      esc.Add (_agentId, escInit.AgentType, escInit.IsCollidable, dim);
      esc.SetPosition(_agentId, VectorToStruct(Position), TVector.UnitVectorXAxis);
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
    ///   Calculate the yaw to a given heading.
    /// </summary>
    /// <param name="target">The target to get orientation to.</param>
    /// <returns>The yaw (corrected to 0 - lt. 360). </returns>
    protected float CalculateYawToTarget(TVector target) {
      var yaw = Yaw;
      var distX = target.X - Position.X;
      var distY = target.Y - Position.Y;

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
    ///   [L0] Perform the movement action. Sends updated values to ESC and receives success or failure.
    /// </summary>
    public void Move() {

      // ESC needs direction vector. So it shall get it. 
      var pitchRad = Pitch * 0.0174532925f;  // Deg -> Rad.
      var yawRad   = Yaw   * 0.0174532925f;      
      var dv = new TVector((float) (Math.Cos(pitchRad) * Math.Cos(yawRad)),
                           (float) (Math.Cos(pitchRad) * Math.Sin(yawRad)),
                           (float) (Math.Sin(pitchRad))).Normalize();      

      // Call ESC movement update and apply returning objects.
      var result = _esc.SetPosition(_agentId, VectorToStruct(TargetPos), dv);
      Position.X = result.Position.X;
      Position.Y = result.Position.Y;
      Position.Z = result.Position.Z;
      //TODO Richtung auch übernehmen, Parameterliste durchreichen an Wahrnehmungsspeicher.
    }


    /// <summary>
    ///   Transform internal vector class to common structure (for transportation).
    /// </summary>
    /// <param name="vector">The vector to send.</param>
    /// <returns>Vector structure. Apart from call-by-value it's identical.</returns>
    protected static TVector VectorToStruct(Vector vector) {
      return new TVector(vector.X, vector.Y, vector.Z);
    }
  }





  /// <summary>
  ///   This structure holds all agent relevant data for registration at the ESC.
  ///   It is passed through from movement module creation down to ML0.
  /// </summary>
  public struct ESCInitData {
    public int AgentId;
    public int AgentType;
    public bool IsCollidable;
  };
}
