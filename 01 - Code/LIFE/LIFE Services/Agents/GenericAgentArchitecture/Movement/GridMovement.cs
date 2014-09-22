using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CommonTypes.DataTypes;
using ESCTestLayer.Interface;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   Direction enumeration for grid movement.
  /// </summary>
  public enum Direction { Up, Right, Down, Left, UpRight, DownRight, DownLeft, UpLeft }


  /// <summary>
  ///   This class enables grid-style movement.
  /// </summary>
  public class GridMovement : ML0 {

    private readonly bool _diagonalEnabled;
    private readonly bool _failureCostEnabled;

    public GridMovement(IESC esc, int agentId, Vector dim) : base(esc, agentId, dim) {
      _diagonalEnabled = true;
      _failureCostEnabled = true;
    }


    /// <summary>
    ///   Perform grid-based movement. Be aware of our axis definition (X: up-down, Y: left-right!. 
    /// </summary>
    /// <param name="direction">The direction to move (enumeration value).</param>
    public void Move(Direction direction) {
      switch (direction) {
        case Direction.Up       : TargetPos = new Vector(TargetPos.X+1, TargetPos.Y);   SetYaw(  0); break;
        case Direction.Down     : TargetPos = new Vector(TargetPos.X-1, TargetPos.Y);   SetYaw(180); break;
        case Direction.Left     : TargetPos = new Vector(TargetPos.X, TargetPos.Y-1);   SetYaw(270); break;
        case Direction.Right    : TargetPos = new Vector(TargetPos.X, TargetPos.Y+1);   SetYaw( 90); break;
        case Direction.UpLeft   : TargetPos = new Vector(TargetPos.X+1, TargetPos.Y-1); SetYaw(  0); break;  //315 | Diagonal 
        case Direction.UpRight  : TargetPos = new Vector(TargetPos.X+1, TargetPos.Y+1); SetYaw(  0); break;  //45  | placement
        case Direction.DownLeft : TargetPos = new Vector(TargetPos.X-1, TargetPos.Y-1); SetYaw(180); break;  //225 | causes
        case Direction.DownRight: TargetPos = new Vector(TargetPos.X-1, TargetPos.Y+1); SetYaw(180); break;  //135 | overlapping!   
      }
      Move();  // Execute movement (call to L0-Move()).
    }


    /// <summary>
    ///   This indirection of base Move() is used to hide it from outside use.
    /// </summary>
    private new void Move() {
      base.Move();
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
        
        // Hide diagonal movement options, if movement points are less than √2 (1.4142).
        var diagonalEnabled = _diagonalEnabled;
        if (movementPoints < 1.4142f) diagonalEnabled = false;

        // Calculate yaw to target position and create sorted list of movement options.
        var angle = CalculateYawToTarget(targetPos);
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
          list.Add(new DirDiff {dir = (Direction) iEnum, diff = diff});
        }

        // Now we have a list of available movement options, ordered by efficiency.
        list.Sort();
        
        // Move it! The loop is needed for alternative action selection.
        for (var option = 0; option < list.Count; option++) {
          var oldPosX = (int) Position.X;
          var oldPosY = (int) Position.Y;
          var dir = list[option].dir;
          
          Console.WriteLine("\nTrying to move from "+Position+" in direction "+dir+": ");
          Move(dir);
          var success = !((int) Position.X == oldPosX && (int) Position.Y == oldPosY);

          // Reduce movement points needed for execution (if operation succeded or failure cost enabled).
          if (success || _failureCostEnabled) {
            if ((int) dir < 4) movementPoints -= 1f;      // straight
            else               movementPoints -= 1.4142f; // diagonal               
          }

          // Did it work? Then go back to main movement function loop.
          if (success) {         
            Console.WriteLine("Movement succeeded.");
            break;
          }
          
          // We're still at the same position. Retry with alternative option. 
          Console.WriteLine("Movement failed. MP left: "+movementPoints);
          if (movementPoints < 1.4142f) break;        
        }

        /*
        //TODO Entwicklungsstand / Abbruchspunkt!
        var le = list.GetEnumerator();  
        Console.WriteLine("\n - Soll-Gierwert: "+angle+"°\n - Diagonalbew.: "+
                          diagonalEnabled+"\n - in Aufl.: "+list.Count+"\n");
        while (le.MoveNext()) Console.WriteLine(le.Current.diff + "°  -  " + le.Current.dir);       
        return;
        */
      }
    }


    /// <summary>
    ///   This structure holds a movement option candidate (combination of direction and difference).
    /// </summary>
    private struct DirDiff : IComparable {
      public Direction dir;
      public float diff;

      public int CompareTo(Object obj) {
        var other = (DirDiff) obj;
        if (diff < other.diff) return -1;
        if (diff > other.diff) return  1;
        return 0;
      }
    }
  }
}
