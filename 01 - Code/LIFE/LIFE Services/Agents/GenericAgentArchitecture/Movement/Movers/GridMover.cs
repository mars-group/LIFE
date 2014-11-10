using System;
using System.Collections.Generic;
using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Movement.Actions;

namespace DalskiAgent.Movement.Movers {

  /// <summary>
  ///   Direction enumeration for grid movement.
  /// </summary>
  public enum GridDir { Up, Right, Down, Left, UpRight, DownRight, DownLeft, UpLeft }
  
  /// <summary>
  ///   This class enables grid-style movement.
  /// </summary>
  public class GridMover : AgentMover {

    private readonly bool _diagonalEnabled;     // This flag enables diagonal movement. 


    /// <summary>
    ///   Create a class for grid movement.
    /// </summary>
    /// <param name="env">Environment interaction interface.</param>
    /// <param name="agent">Agent reference, needed for movement execution.</param>
    /// <param name="data">Container with spatial base data.</param>
    /// <param name="diagonal">'True': diagonal movement enabled (default value). 
    /// On 'false', only up, down, left and right are available.</param>
    public GridMover(IEnvironment env, SpatialAgent agent, MovementData data, bool diagonal = true) : base(env, agent, data) {
      _diagonalEnabled = diagonal;
    }


    /// <summary>
    ///   Perform grid-based movement. Be aware of our axis definition (X: up-down, Y: left-right)! 
    /// </summary>
    /// <param name="direction">The direction to move (enumeration value).</param>
    public void Move(GridDir direction) {
      TargetPos = new Vector(Data.Position.X, Data.Position.Y);
      TargetDir = new Direction();
      switch (direction) {
        case GridDir.Up       : TargetPos.X ++;                  TargetDir.SetYaw(  0);  break;
        case GridDir.Down     : TargetPos.X --;                  TargetDir.SetYaw(180);  break;
        case GridDir.Left     :                 TargetPos.Y --;  TargetDir.SetYaw(270);  break;
        case GridDir.Right    :                 TargetPos.Y ++;  TargetDir.SetYaw( 90);  break;
        case GridDir.UpLeft   : TargetPos.X ++; TargetPos.Y --;  TargetDir.SetYaw(  0);  break;  //315 | Diagonal 
        case GridDir.UpRight  : TargetPos.X ++; TargetPos.Y ++;  TargetDir.SetYaw(  0);  break;  //45  | placement
        case GridDir.DownLeft : TargetPos.X --; TargetPos.Y --;  TargetDir.SetYaw(180);  break;  //225 | causes
        case GridDir.DownRight: TargetPos.X --; TargetPos.Y ++;  TargetDir.SetYaw(180);  break;  //135 | overlapping!   
      }
      Move();  // Execute movement (call to L0-Move()).
    }


    /// <summary>
    ///   Create a movement action towards a given position using grid options. 
    /// </summary>
    /// <param name="direction">The direction to move.</param>
    /// <returns>A movement action.</returns>
    public GridMovementAction MoveInDirection(GridDir direction) {
      return new GridMovementAction(this, direction);
    }


    /// <summary>
    ///   Get the movement options towards a given position.
    /// </summary>
    /// <param name="targetPos">The target position.</param>
    /// <returns>A list of available movement options. These are ordered 
    /// by angular offset to optimal heading (sorting value of struct).</returns>
    public List<MovementOption> GetMovementOptions(Vector targetPos) {

      // Check, if we are already there. Otherwise no need to move anyway (empty list).
      if (targetPos.Equals(Data.Position)) return new List<MovementOption>();

      // Calculate yaw to target position and create sorted list of movement options.
      var angle = CalculateDirectionToTarget(targetPos).Yaw;
      var list = new List<MovementOption>();

      // Add directions enum values and angular differences to list.
      // We loop over all options and calculate difference between desired and actual value.
      for (int iEnum = 0, offset = 0, mod = 0; iEnum < 8; iEnum ++, mod ++) {
        
        // If diagonal movement is allowed, set offset and continue. Otherwise abort.
        if (iEnum == 4) {
          if (_diagonalEnabled) { offset = 45; mod = 0; }
          else break;
        }

        // Calculate angular difference to current option. If >180°, consider other semicircle.
        var diff = Math.Abs(angle - (offset + mod*90));
        if (diff > 180.0f) diff = 360.0f - diff;
        list.Add(new MovementOption {Direction = (GridDir) iEnum, Offset = diff});
      }

      // Now we have a list of available movement options, ordered by efficiency.
      list.Sort();
      return list;
    } 


    /// <summary>
    ///   This structure holds a movement option candidate (combination of direction and difference).
    /// </summary>
    public struct MovementOption : IComparable {
      public GridDir Direction;
      public float Offset;

      public int CompareTo(Object obj) {
        var other = (MovementOption) obj;
        if (Offset < other.Offset) return -1;
        return Offset > other.Offset ? 1 : 0;
      }
    }
  }
}
