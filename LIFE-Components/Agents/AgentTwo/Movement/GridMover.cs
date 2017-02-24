using System;
using System.Collections.Generic;
using LIFE.API.GridCommon;
using LIFE.Components.Agents.AgentTwo.Reasoning;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using GridPosition = LIFE.Components.Agents.AgentTwo.Environment.GridPosition;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LIFE.Components.Agents.AgentTwo.Movement {

  /// <summary>
  ///   Two-dimensional, grid-based movement module.
  ///   For now, this mover rests upon the 2D continuous environment, emulating grid behaviour.
  /// </summary>
  public class GridMover {

    private readonly AgentMover2D _mover2D;   // 2D movement module used internally.
    private readonly GridPosition _pos;       // Current agent grid position.
    public bool DiagonalEnabled { get; set; } // This flag enables diagonal movement [default: disabled].


    /// <summary>
    ///   Create a new grid movement module.
    /// </summary>
    /// <param name="mover2D">2D continuous cartesian mover.</param>
    /// <param name="pos">The agent's position structure.</param>
    public GridMover(AgentMover2D mover2D, GridPosition pos) {
      _mover2D = mover2D;
      _pos = pos;
    }


    /// <summary>
    ///   Try to insert this agent into the environment at the given position.
    /// </summary>
    /// <param name="x">Agent start position (x-coordinate).</param>
    /// <param name="y">Agent start position (y-coordinate).</param>
    /// <returns>Success flag. If failed, the agent may not be moved!</returns>
    public bool InsertIntoEnvironment(int x, int y) {
      return _mover2D.InsertIntoEnvironment(x, y);
    }


    /// <summary>
    ///   Perform grid-based movement.
    /// </summary>
    /// <param name="direction">The direction to move (enumeration value).</param>
    /// <returns>An interaction object that contains the code to execute this movement.</returns>
    public MovementAction MoveInDirection(GridDirection direction) {
      const double r = 1.41421356;
      switch (direction) {
        case GridDirection.Up       : return _mover2D.MoveInDirection(1,   0);
        case GridDirection.UpRight  : return _mover2D.MoveInDirection(r,  45);
        case GridDirection.Right    : return _mover2D.MoveInDirection(1,  90);
        case GridDirection.DownRight: return _mover2D.MoveInDirection(r, 135);
        case GridDirection.Down     : return _mover2D.MoveInDirection(1, 180);
        case GridDirection.DownLeft : return _mover2D.MoveInDirection(r, 225);
        case GridDirection.Left     : return _mover2D.MoveInDirection(1, 270);
        case GridDirection.UpLeft   : return _mover2D.MoveInDirection(r, 315);
        default: return null;
      }
    }


    /// <summary>
    ///   Get the movement options towards a given position.
    /// </summary>
    /// <param name="targetX">Target position x-coordinate.</param>
    /// <param name="targetY">Target position y-coordinate.</param>
    /// <returns>A list of available movement options. These are ordered
    /// by angular offset to optimal heading (sorting value of struct).</returns>
    public List<MovementOption> GetMovementOptions(int targetX, int targetY) {

      // Check, if we are already there. Otherwise no need to move anyway (empty list).
      if (targetX == _pos.X && targetY == _pos.Y) return new List<MovementOption>();

      // Calculate yaw to target position.
      var joint = new Vector3(targetX - _pos.X, targetY - _pos.Y, 0);
      var dir = new Direction();
      dir.SetDirectionalVector(joint);
      var angle = dir.Yaw;

      // Create sortable list of movement options.
      var list = new List<MovementOption>();

      // Add directions enum values and angular differences to list.
      // We loop over all options and calculate difference between desired and actual value.
      for (int iEnum = 0, offset = 0, mod = 0; iEnum < 8; iEnum ++, mod ++) {
        if (iEnum == 4) {                                 //| If diagonal movement is
          if (DiagonalEnabled) { offset = 45; mod = 0; }  //| allowed, set offset and
          else break;                                     //| continue. Otherwise abort.
        }

        // Calculate angular difference to current option. If >180°, consider other semicircle.
        var diff = Math.Abs(angle - (offset + mod*90));
        if (diff > 180.0f) diff = 360.0f - diff;
        list.Add(new MovementOption {Direction = (GridDirection) iEnum, Offset = diff});
      }

      // Now we have a list of available movement options, ordered by efficiency.
      list.Sort();
      return list;
    }
  }



  /// <summary>
  ///   This structure holds a movement option candidate (combination of direction and difference).
  /// </summary>
  public struct MovementOption : IComparable {
    public GridDirection Direction; // The represented grid movement direction.
    public double Offset;     // Angular offset to target (heuristic).


    /// <summary>
    ///   Comparison method to find the option with the smallest offset.
    /// </summary>
    /// <param name="obj">Another movement option.</param>
    /// <returns></returns>
    public int CompareTo(object obj) {
      var other = (MovementOption) obj;
      if (Offset < other.Offset) return -1;
      return Offset > other.Offset ? 1 : 0;
    }
  }
}