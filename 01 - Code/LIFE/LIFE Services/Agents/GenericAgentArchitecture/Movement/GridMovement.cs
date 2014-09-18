﻿using System;
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

    public GridMovement(IESC esc, int agentId, Vector3f dim) : base(esc, agentId, dim) {
      _diagonalEnabled = true;
      _failureCostEnabled = true;
    }


    /// <summary>
    ///   Perform grid-based movement. 
    /// </summary>
    /// <param name="direction">The direction to move (enumeration value).</param>
    public void Move(Direction direction) {
      switch (direction) {
        case Direction.Up       :                 TargetPos.Y ++;  SetYaw(  0);  break;
        case Direction.Down     :                 TargetPos.Y --;  SetYaw(180);  break;
        case Direction.Left     : TargetPos.X --;                  SetYaw(270);  break;
        case Direction.Right    : TargetPos.X ++;                  SetYaw( 90);  break;
        case Direction.UpLeft   : TargetPos.X --; TargetPos.Y ++;  SetYaw(315);  break;
        case Direction.UpRight  : TargetPos.X ++; TargetPos.Y ++;  SetYaw( 45);  break;
        case Direction.DownLeft : TargetPos.X --; TargetPos.Y --;  SetYaw(225);  break;
        case Direction.DownRight: TargetPos.X ++; TargetPos.Y --;  SetYaw(135);  break;     
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
    public void MoveToPosition(Vector2f targetPos, float movementPoints) {

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



        //TODO Entwicklungsstand / Abbruchspunkt!
        var le = list.GetEnumerator();  
        Console.WriteLine("\n - Soll-Gierwert: "+angle+"°\n - Diagonalbew.: "+
                          diagonalEnabled+"\n - in Aufl.: "+list.Count+"\n");
        while (le.MoveNext()) Console.WriteLine(le.Current.diff + "°  -  " + le.Current.dir);       
        return;


        //TODO Was, wenn die Diffs gleich sind? !!
         

        Direction dir = Direction.Down;


        // Reduce available movement points after execution.
        if ((int) dir < 4) movementPoints -= 1f;        // straight
        else               movementPoints -= 1.4142f;   // diagonal 
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
