using DalskiAgent.Agents;
using LifeAPI.Environment;
using LifeAPI.Spatial;

namespace DalskiAgent.Movement.Movers {

  /// <summary>
  ///   L0 class: Enables basic agent movement by direct placement.
  /// </summary>
  public class DirectMover : AgentMover {


    /// <summary>
    ///   Create an agent mover for direct placement.
    /// </summary>
    /// <param name="env">Environment interaction interface.</param>
    /// <param name="agent">Agent reference, needed for movement execution.</param>
    public DirectMover(IEnvironment env, SpatialAgent agent) : base(env, agent) {}


    /// <summary>
    ///   Execute movement. This is called by movement action in phase III.
    /// </summary>
    /// <param name="target">The target position.</param>
    /// <param name="dir">The new direction (optional).</param> 
    public void Move(Vector target, Direction dir = null) {
      MovementVector = new Vector(
        target.X - Agent.GetPosition().X, 
        target.Y - Agent.GetPosition().Y, 
        target.Z - Agent.GetPosition().Z);
      if (dir != null) TargetDir = dir;
      else TargetDir = Agent.GetDirection();
      Move();
    }
  }
}
