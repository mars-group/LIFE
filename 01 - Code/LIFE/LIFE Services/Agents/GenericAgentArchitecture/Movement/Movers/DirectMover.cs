using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Environments;

namespace GenericAgentArchitecture.Movement.Movers {

  /// <summary>
  ///   L0 class: Enables basic agent movement by direct placement.
  /// </summary>
  public class DirectMover : AgentMover {


    /// <summary>
    ///   Create an agent mover for direct placement.
    /// </summary>
    /// <param name="env">Environment interaction interface.</param>
    /// <param name="agent">Agent reference, needed for movement execution.</param>
    /// <param name="data">Container with spatial base data.</param>
    public DirectMover(IEnvironment env, SpatialAgent agent, MovementData data) : base(env, agent, data) {}


    /// <summary>
    ///   Execute movement. This is called by movement action in phase III.
    /// </summary>
    /// <param name="target">The target position.</param>
    /// <param name="dir">The new direction (optional).</param> 
    public void Move(Vector target, Direction dir = null) {
      TargetPos = new Vector(target.X, target.Y, target.Z);
      if (dir != null) TargetDir = dir;
      else TargetDir = Data.Direction;
      Move();
    }
  }
}
