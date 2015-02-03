using DalskiAgent.Agents;
using LifeAPI.Environment;
using LifeAPI.Spatial;
using SpatialCommon.Transformation;

namespace DalskiAgent.Movement.Movers {

  /// <summary>
  ///   This abstract class serves as a base for agent movement.
  ///   It interfaces an IEnvironment implementation.
  /// </summary>
  public abstract class AgentMover {

    private readonly IEnvironment _env;    // Environment interaction interface.
    protected readonly SpatialAgent Agent; // Agent reference, needed for movement execution.  
    protected Vector3 MovementVector;      // Target position to acquire. May be set or calculated.
    protected Direction TargetDir;         // Desired heading.
    public static float TickLength = 1.0f; // Timelength of a simulation tick.
    public MovementResult MovementResult;  // Result of last movement.


    /// <summary>
    ///   Instantiate a new base L0 class. Only available for specializations.
    /// </summary>
    /// <param name="env">Environment interaction interface.</param>
    /// <param name="agent">Agent reference, needed for movement execution.</param>
    protected AgentMover(IEnvironment env, SpatialAgent agent) {
      _env = env;
      Agent = agent;
      MovementResult = null;
    }


    /// <summary>
    ///   [L0] Perform the movement action. Call environment interface with updated values.
    ///   The adapter is responsible to set the checked (returned) data.
    /// </summary>
    protected void Move() {
      MovementResult = _env.Move(Agent, MovementVector, TargetDir);
    }


    /// <summary>
    ///   Calculate the needed direction towards a given position.
    /// </summary>
    /// <param name="target">The target to get orientation to.</param>
    /// <returns>The yaw (corrected to 0 - lt. 360). </returns>
    public Direction CalculateDirectionToTarget(Vector3 target) {
      var diff = new Vector3(target.X - Agent.GetPosition().X, 
                             target.Y - Agent.GetPosition().Y,
                             target.Z - Agent.GetPosition().Z);
      
      // Create new direction, set joint vector as reference and return.
      var dir = new Direction();
      dir.SetDirectionalVector(diff);
      return dir;
    }
  }
}
