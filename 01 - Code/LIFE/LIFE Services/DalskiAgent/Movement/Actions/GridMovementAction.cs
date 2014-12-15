using System;
using DalskiAgent.Movement.Movers;
using DalskiAgent.Reasoning;

namespace DalskiAgent.Movement.Actions {

  /// <summary>
  ///   This class enables grid-style movement.
  /// </summary>
  public class GridMovementAction : IInteraction {

    private readonly GridMover _mover; // The agent movement module.
    private readonly GridDir _gridDir; // The grid direction to move.


    /// <summary>
    ///   Create a movement action for grid-based movement in 2D environments.
    /// </summary>
    /// <param name="mover">The agent movement module.</param>
    /// <param name="gridDir">The grid direction to move.</param>
    public GridMovementAction(AgentMover mover, GridDir gridDir) {
      if (mover is GridMover) _mover = (GridMover) mover;
      else throw new Exception("[GridMovementAction] Error: Agent movement type is incompatible!");
      _gridDir = gridDir;
    }


    /// <summary>
    ///   Execute movement on grid mover class.
    /// </summary>
    public void Execute() {
      _mover.Move(_gridDir);
    }
  }
}

