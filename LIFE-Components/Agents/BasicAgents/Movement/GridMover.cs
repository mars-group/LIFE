using System;
using System.Collections.Generic;
using LIFE.API.GridCommon;
using LIFE.Components.Agents.BasicAgents.Perception;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.Environments.GridEnvironment;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace LIFE.Components.Agents.BasicAgents.Movement {

  /// <summary>
  ///   Two-dimensional, grid-based movement module.
  ///   For now, this mover rests upon the 2D continuous environment, emulating grid behaviour.
  /// </summary>
  public class GridMover : AgentMover {

    private readonly IGridEnvironment<IGridCoordinate> _grid; // The grid environment to use.
    private readonly GridPosition _position;                  // Agent position structure.
    private bool _isInserted;                                 // Is this agent already inserted?

    public bool DiagonalEnabled { get; set; } // This flag enables diagonal movement [default: disabled].


    /// <summary>
    ///   Create a new grid movement module.
    /// </summary>
    /// <param name="env">The grid environment to use.</param>
    /// <param name="pos">Agent position data structure.</param>
    /// <param name="sensorArray">The agent's sensor array (to provide movement feedback).</param>
    public GridMover(IGridEnvironment<IGridCoordinate> env, GridPosition pos, SensorArray sensorArray)
      : base(sensorArray) {
      _grid = env;
      _position = pos;
    }


    /// <summary>
    ///   Try to insert this agent into the environment at the given position.
    /// </summary>
    /// <param name="x">Agent start position (x-coordinate).</param>
    /// <param name="y">Agent start position (y-coordinate).</param>
    public void InsertIntoEnvironment(int x, int y) {
      if (!_isInserted) {
        _position.X = x;
        _position.Y = y;
        _grid.Insert(_position);
        _isInserted = true;
      }
    }


    /// <summary>
    ///   Perform grid-based movement.
    /// </summary>
    /// <param name="dir">The direction to move (enumeration value).</param>
    /// <returns>An interaction object that contains the code to execute this movement.</returns>
    public MovementAction MoveInDirection(GridDirection dir) {
      var x = _position.X;
      var y = _position.Y;
      switch (dir) {
        case GridDirection.Up       : return SetToPosition(x,   y+1, dir);
        case GridDirection.UpRight  : return SetToPosition(x+1, y+1, dir);
        case GridDirection.Right    : return SetToPosition(x+1, y,   dir);
        case GridDirection.DownRight: return SetToPosition(x+1, y-1, dir);
        case GridDirection.Down     : return SetToPosition(x,   y-1, dir);
        case GridDirection.DownLeft : return SetToPosition(x-1, y-1, dir);
        case GridDirection.Left     : return SetToPosition(x-1, y,   dir);
        case GridDirection.UpLeft   : return SetToPosition(x-1, y+1, dir);
        default: return null;
      }
    }


    /// <summary>
    ///   Set this agent to a new position.
    /// </summary>
    /// <param name="x">X coordinate to move to.</param>
    /// <param name="y">Y coordinate to move to.</param>
    /// <param name="dir">Agent orientation (optional).</param>
    /// <returns></returns>
    public MovementAction SetToPosition(int x, int y, GridDirection dir = GridDirection.NotSet) {
      if (!_isInserted) return null;
      return new MovementAction(() => {
        var result = _grid.MoveToPosition(_position, x, y);
        if (dir != GridDirection.NotSet) _position.GridDirection = dir;
        if (result.X == _position.X && result.Y == _position.Y) {
          MovementSensor.SetMovementResult(new MovementResult(MovementStatus.OutOfBounds));
        }
        else {
          _position.X = result.X;
          _position.Y = result.Y;
          MovementSensor.SetMovementResult(new MovementResult(MovementStatus.Success));                
        }
      });
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
      if (targetX == _position.X && targetY == _position.Y) return new List<MovementOption>();

      // Calculate yaw to target position.
      var joint = new Vector3(targetX - _position.X, targetY - _position.Y, 0);
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
    public double Offset;           // Angular offset to target (heuristic).


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