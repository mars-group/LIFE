using System;
using System.Collections.Generic;
using GenericAgentArchitecture.Agents;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   Direction enumeration for grid movement.
  /// </summary>
  public enum GridDir { Up, Right, Down, Left, UpRight, DownRight, DownLeft, UpLeft }
  
  /// <summary>
  ///   This class enables grid-style movement.
  /// </summary>
  public class GridMover : AgentMover {

    private readonly bool _diagonalEnabled;     // This flag enables diagonal movement. 
    private readonly bool _failureCostEnabled;  // If set to 'true', failed movements also cost movement points.


    /// <summary>
    ///   Create a class for grid movement.
    /// </summary>
    /// <param name="env">Environment interaction interface.</param>
    /// <param name="agent">Agent reference, needed for movement execution.</param>
    /// <param name="data">Container with spatial base data.</param>
    public GridMover(IEnvironment env, SpatialAgent agent, MData data) : base(env, agent, data) {
      _diagonalEnabled = true;
      _failureCostEnabled = true;
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
    ///   Moves towards a given position using grid options. 
    /// </summary>
    /// <param name="targetPos">The target position to move to.</param>
    /// <param name="movementPoints">The distance the agent is allowed to travel.
    /// Similar to the speed in continuous environments.</param>
    public void MoveToPosition(Vector targetPos, float movementPoints) {

      // Repeat function as long as movement is possible (minimal cost: 1).
      while (movementPoints >= 1) {
         
        // Check, if we are already there. Otherwise no need to move anyway.
        if (targetPos.Equals(Data.Position)) return;
        
        // Hide diagonal movement options, if movement points are less than √2 (1.4142).
        var diagonalEnabled = _diagonalEnabled;
        if (movementPoints < Sqrt2) diagonalEnabled = false;

        // Calculate yaw to target position and create sorted list of movement options.
        var angle = CalculateDirectionToTarget(targetPos).Yaw;
        var list = new List<DirDiff>();

        // Add directions enum values and angular differences to list.
        // We loop over all options and calculate difference between desired and actual value.
        for (int iEnum = 0, offset = 0, mod = 0; iEnum < 8; iEnum ++, mod ++) {
        
          // If diagonal movement is allowed, set offset and continue. Otherwise abort.
          if (iEnum == 4) {
            if (diagonalEnabled) { offset = 45; mod = 0; }
            else break;
          }

          // Calculate angular difference to current option. If >180°, consider other semicircle.
          var diff = Math.Abs(angle - (offset + mod*90));
          if (diff > 180.0f) diff = 360.0f - diff;
          list.Add(new DirDiff {Dir = (GridDir) iEnum, Diff = diff});
        }

        // Now we have a list of available movement options, ordered by efficiency.
        list.Sort();
        
        // Move it! The loop is needed for alternative action selection.
        for (var option = 0; option < list.Count; option++) {
          var oldPosX = (int) Data.Position.X;
          var oldPosY = (int) Data.Position.Y;
          var dir = list[option].Dir;
          
          Console.WriteLine("\nTrying to move from "+Data.Position+" in direction "+dir+": ");
          Move(dir);
          var success = !((int) Data.Position.X == oldPosX && (int) Data.Position.Y == oldPosY);

          // Reduce movement points needed for execution (if operation succeded or failure cost enabled).
          if (success || _failureCostEnabled) {
            if ((int) dir < 4) movementPoints -= 1f;    // straight
            else               movementPoints -= Sqrt2; // diagonal               
          }

          // Did it work? Then go back to main movement function loop.
          if (success) {         
            Console.WriteLine("Movement succeeded.");
            break;
          }
          
          // We're still at the same position. Retry with alternative option. 
          Console.WriteLine("Movement failed. MP left: "+movementPoints);

          if (TargetPos.Equals(targetPos)) return;   // If final destination is blocked, abort pathfinding.      
          TargetPos = new Vector(oldPosX, oldPosY);  // Reset target origin.
          if (movementPoints < Sqrt2) break;         // Break, if no options left.
          //TODO Schrägbewegung wäre noch möglich !!
        }
      }   
    }


    /// <summary>
    ///   This structure holds a movement option candidate (combination of direction and difference).
    /// </summary>
    private struct DirDiff : IComparable {
      public GridDir Dir;
      public float Diff;

      public int CompareTo(Object obj) {
        var other = (DirDiff) obj;
        if (Diff < other.Diff) return -1;
        return Diff > other.Diff ? 1 : 0;
      }
    }
  }
}
