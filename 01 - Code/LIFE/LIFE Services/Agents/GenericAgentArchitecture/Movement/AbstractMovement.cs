using System;
using GenericAgentArchitecture.Agents;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   This abstract class serves as a base for agent movement.
  ///   It interfaces the Environment Service Component (ESC).
  /// </summary>
  internal class AbstractMovement {

    private Vector _targetPos;   // Target position. May be set or auto-calculated.
    private SpatialAgent _agent; // The agent who shall be moved.


    //TODO PROBLEM: Nur die Aktion soll Werte ändern dürfen.
    //TODO Bei "public" kann es jeder - auch andere Agenten!!
    // Doch wieder Position-Klasse, mit x, y, z, usw.
    // Kann der IA im Konstr. übergeben werden
    // ... und im Agenten selbst protected bleiben






    /// <summary>
    ///   [L0] Perform the movement action. Sends updated values to ESC and receives success or failure.
    /// </summary>
    public void Move() {

      // ESC needs direction vector. So it shall get it. 
      var pitchRad = _agent.Pitch * 0.0174532925f;  // Deg -> Rad.
      var yawRad   = _agent.Yaw   * 0.0174532925f;      
      var dv = new TVector((float) (Math.Cos(pitchRad) * Math.Cos(yawRad)),
                           (float) (Math.Cos(pitchRad) * Math.Sin(yawRad)),
                           (float) (Math.Sin(pitchRad))).Normalize();      

      // Call ESC movement update and apply returning objects.
      var result = _agent..SetPosition(_agentId, VectorToStruct(TargetPos), dv);
      _agent.Position.X = result.Position.X;
      _agent.Position.Y = result.Position.Y;
      _agent.Position.Z = result.Position.Z;
      //TODO Richtung auch übernehmen, Parameterliste durchreichen an Wahrnehmungsspeicher.
    }


  }
}