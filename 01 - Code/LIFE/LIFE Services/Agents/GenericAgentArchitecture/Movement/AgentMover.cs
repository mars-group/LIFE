using ESCTestLayer.Interface;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   This abstract class serves as a base for agent movement.
  ///   It interfaces the Environment Service Component (ESC).
  /// </summary>
  public abstract class AgentMover {

    private readonly IESC _esc;    // ESC interface for collision detection.
    private readonly int _id;      // Agent identifier, needed for ESC registration.
    protected readonly MData Data; // The agent's movement data container.
    
    protected Vector TargetPos;    // Target position to acquire. May be set or calculated.
    protected Direction TargetDir; // Desired heading.

    public const float Sqrt2   = 1.4142f;    // The square root of 2.
    public static float TickLength = 1.0f;   // Timelength of a simulation tick.


    /// <summary>
    ///   Instantiate a new base L0 class. Only available for specializations.
    /// </summary>
    /// <param name="esc">IESC implemenation reference.</param>
    /// <param name="id">Agent identifier, needed by ESC.</param>
    /// <param name="data">Container with spatial base data.</param>
    protected AgentMover(IESC esc, int id, MData data) {
      _esc = esc;
      _id = id;
      Data = data;
    }


    /// <summary>
    ///   [L0] Perform the movement action. Sends updated values to ESC and receives success or failure.
    /// </summary>
    protected void Move() {
      var dv = TargetDir.GetDirectionalVector();
      var dt = new TVector(dv.X, dv.Y, dv.Z);
      var pt = new TVector(TargetPos.X, TargetPos.Y, TargetPos.Z);

      // Call ESC movement update and apply returning objects.
      var result = _esc.SetPosition(_id, pt, dt);
      Data.Position.X = result.Position.X;
      Data.Position.Y = result.Position.Y;
      Data.Position.Z = result.Position.Z;
      //TODO Richtung auch übernehmen. Gibt die ESC noch nicht her!
      //Data.Direction.SetDirectionalVector(result ...);
      Data.Direction.SetPitch(TargetDir.Pitch);
      Data.Direction.SetYaw(TargetDir.Yaw);
      //TODO Parameterliste durchreichen an Wahrnehmungsspeicher.
    }


    /// <summary>
    ///   Calculate the needed direction towards a given position.
    /// </summary>
    /// <param name="target">The target to get orientation to.</param>
    /// <returns>The yaw (corrected to 0 - lt. 360). </returns>
    public Direction CalculateDirectionToTarget(Vector target) {
      var diff = new Vector(target.X - Data.Position.X, 
                            target.Y - Data.Position.Y,
                            target.Z - Data.Position.Z);
      
      // Create new direction, set joint vector as reference and return.
      var dir = new Direction();
      dir.SetDirectionalVector(diff);
      return dir;
    }
  }
}
