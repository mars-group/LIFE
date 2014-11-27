using DalskiAgent.Agents;
using DalskiAgent.Environments;
using GenericAgentArchitectureCommon.Datatypes;
using SpatialCommon.Datatypes;

namespace DalskiAgent.Movement.Movers {

  /// <summary>
  ///   This abstract class serves as a base for agent movement.
  ///   It interfaces an IEnvironment implementation.
  /// </summary>
  public abstract class AgentMover {

    private readonly IEnvironment _env;    // Environment interaction interface.
    private readonly SpatialAgent _agent;  // Agent reference, needed for movement execution.
    protected readonly DataAccessor Data;  // The agent's movement data container.
   
    protected Vector MovementVector;       // Target position to acquire. May be set or calculated.
    protected Direction TargetDir;         // Desired heading.

    public const float Sqrt2 = 1.4142f;    // The square root of 2.
    public static float TickLength = 1.0f; // Timelength of a simulation tick.


    /// <summary>
    ///   Instantiate a new base L0 class. Only available for specializations.
    /// </summary>
    /// <param name="env">Environment interaction interface.</param>
    /// <param name="agent">Agent reference, needed for movement execution.</param>
    /// <param name="data">R/O container for spatial data.</param>
    protected AgentMover(IEnvironment env, SpatialAgent agent, DataAccessor data) {
      _env = env;
      _agent = agent;
      Data = data;
    }


    /// <summary>
    ///   [L0] Perform the movement action. Call environment interface with updated values.
    ///   The adapter is responsible to set the checked (returned) data.
    /// </summary>
    protected void Move() {
      _env.MoveObject(_agent, MovementVector, TargetDir);
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
