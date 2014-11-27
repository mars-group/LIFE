using DalskiAgent.Agents;
using DalskiAgent.Environments;
using GenericAgentArchitectureCommon.Datatypes;
using SpatialCommon.Datatypes;

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
    /// <param name="data">R/O container for spatial data.</param>
    public DirectMover(IEnvironment env, SpatialAgent agent, DataAccessor data) : base(env, agent, data) {}


    /// <summary>
    ///   Execute movement. This is called by movement action in phase III.
    /// </summary>
    /// <param name="target">The target position.</param>
    /// <param name="dir">The new direction (optional).</param> 
    public void Move(Vector target, Direction dir = null) {
      MovementVector = new Vector(
        target.X-Data.Position.X, 
        target.Y-Data.Position.Y, 
        target.Z-Data.Position.Z);
      if (dir != null) TargetDir = dir;
      else TargetDir = Data.Direction;
      Move();
    }
  }
}
