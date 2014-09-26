using ESCTestLayer.Interface;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   This abstract class serves as a base for agent movement.
  ///   It interfaces the Environment Service Component (ESC).
  /// </summary>
  public abstract class AbstractMovement {

    private IESC _iesc;         // ESC interface for collision detection.
    protected MData Data;       // The agent's movement data container.
    protected Vector TargetPos; // Target position to acquire. May be set or calculated.
    protected float TickLength; // Timelength of a simulation tick.

    protected AbstractMovement() {
      //TODO Epic stuff ...
      TickLength = MovementServices.TickLength;
    }


    /// <summary>
    ///   [L0] Perform the movement action. Sends updated values to ESC and receives success or failure.
    /// </summary>
    public void Move() {
      var dv = MovementServices.GetDirectionalVector(Data);
      var tp = new TVector(TargetPos.X, TargetPos.Y, TargetPos.Z);

      // Call ESC movement update and apply returning objects.
      var result = _iesc.SetPosition(_agentId, tp, dv);
      Data.Position.X = result.Position.X;
      Data.Position.Y = result.Position.Y;
      Data.Position.Z = result.Position.Z;
      //TODO Richtung auch übernehmen, Parameterliste durchreichen an Wahrnehmungsspeicher.
    }
  }
}