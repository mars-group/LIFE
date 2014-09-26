using ESCTestLayer.Interface;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   L0 class: Enables basic agent movement by direct placement.
  /// </summary>
  public class DirectMover : AgentMover {


    /// <summary>
    ///   Create an agent mover for direct placement.
    /// </summary>
    /// <param name="esc">IESC implemenation reference.</param>
    /// <param name="id">Agent identifier, needed by ESC.</param>
    /// <param name="data">Container with spatial base data.</param>
    public DirectMover(IESC esc, int id, MData data) : base(esc, id, data) {}


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
