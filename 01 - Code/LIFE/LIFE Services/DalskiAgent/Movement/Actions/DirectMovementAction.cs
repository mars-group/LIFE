using System;
using DalskiAgent.Movement.Movers;
using DalskiAgent.Reasoning;
using SpatialAPI.Entities.Transformation;

namespace DalskiAgent.Movement.Actions {
  
  /// <summary>
  ///   Interaction for direct agent placement.
  /// </summary>
  public class DirectMovementAction : IInteraction {

    private readonly DirectMover _mover;    // The agent movement module.
    private readonly Vector3 _targetPos;    // The new target position.
    private readonly Direction _targetDir;  // The new direction (optional).


    /// <summary>
    ///   Create a direct movement action.
    /// </summary>
    /// <param name="mover">The agent movement module.</param>
    /// <param name="position">The new target position.</param>
    /// <param name="dir">The new direction (optional).</param>
    public DirectMovementAction(AgentMover mover, Vector3 position, Direction dir = null) {
      if (mover is DirectMover) _mover = (DirectMover) mover;
      else throw new Exception("[DirectMovementAction] Error: Agent movement type is incompatible!");
      _targetPos = position;
      _targetDir = dir;
    }


    /// <summary>
    ///   Call agent mover to execute movement.
    /// </summary>
    public void Execute() {
      _mover.Move(_targetPos, _targetDir);
    }
  }
}