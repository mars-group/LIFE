﻿using System;
using System.Collections.Generic;
using ESCTestLayer.Interface;
using GenericAgentArchitectureCommon.Interfaces;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   Direction enumeration for grid movement.
  /// </summary>
  public enum Direction { Up, Right, Down, Left, UpRight, DownRight, DownLeft, UpLeft }


  /// <summary>
  ///   This class enables grid-style movement.
  /// </summary>
  public class GridMovement : AbstractMovement, IInteraction {
    
    private const float Sqrt2 = 1.4142f;        // The square root of 2.
    private readonly bool _diagonalEnabled;     // This flag enables diagonal movement. 
    private readonly bool _failureCostEnabled;  // If set to 'true', failed movements also cost movement points.
   

    /// <summary>
    ///   Create a class for grid movement.
    /// </summary>
    /// <param name="esc">IESC implemenation reference.</param>
    /// <param name="escInit">Initialization data needed by ESC.</param>
    /// <param name="pos">Agent's initial position.</param>
    /// <param name="dim">Agent's physical dimension.</param>
    public GridMovement(IESC esc, ESCInitData escInit, TVector pos, TVector dim) : base(esc, escInit, pos, dim) {
      _diagonalEnabled = true;
      _failureCostEnabled = true;
    }


    /// <summary>
    ///   Perform grid-based movement. Be aware of our axis definition (X: up-down, Y: left-right!. 
    /// </summary>
    /// <param name="direction">The direction to move (enumeration value).</param>
    public void Move(Direction direction) {
      switch (direction) {
        case Direction.Up       : TargetPos.X ++;                  SetYaw(  0);  break;
        case Direction.Down     : TargetPos.X --;                  SetYaw(180);  break;
        case Direction.Left     :                 TargetPos.Y --;  SetYaw(270);  break;
        case Direction.Right    :                 TargetPos.Y ++;  SetYaw( 90);  break;
        case Direction.UpLeft   : TargetPos.X ++; TargetPos.Y --;  SetYaw(  0);  break;  //315 | Diagonal 
        case Direction.UpRight  : TargetPos.X ++; TargetPos.Y ++;  SetYaw(  0);  break;  //45  | placement
        case Direction.DownLeft : TargetPos.X --; TargetPos.Y --;  SetYaw(180);  break;  //225 | causes
        case Direction.DownRight: TargetPos.X --; TargetPos.Y ++;  SetYaw(180);  break;  //135 | overlapping!   
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
    public void MoveToPosition(TVector targetPos, float movementPoints) {

      // Repeat function as long as movement is possible (minimal cost: 1).
      while (movementPoints >= 1) {
         
        // Check, if we are already there. Otherwise no need to move anyway.
        if (((int)targetPos.X == (int)Position.X) && ((int)targetPos.Y == (int)Position.Y)) return;
        
        // Hide diagonal movement options, if movement points are less than √2 (1.4142).
        var diagonalEnabled = _diagonalEnabled;
        if (movementPoints < Sqrt2) diagonalEnabled = false;

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
          list.Add(new DirDiff {Dir = (Direction) iEnum, Diff = diff});
        }

        // Now we have a list of available movement options, ordered by efficiency.
        list.Sort();
        
        // Move it! The loop is needed for alternative action selection.
        for (var option = 0; option < list.Count; option++) {
          var oldPosX = (int) Position.X;
          var oldPosY = (int) Position.Y;
          var dir = list[option].Dir;
          
          Console.WriteLine("\nTrying to move from "+Position+" in direction "+dir+": ");
          Move(dir);
          var success = !((int) Position.X == oldPosX && (int) Position.Y == oldPosY);

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

          if (VectorToStruct(TargetPos) == targetPos) return; // If final destination is blocked, abort pathfinding.      
          TargetPos = new Vector(oldPosX, oldPosY);           // Reset target origin.
          if (movementPoints < Sqrt2) break;                  // Break, if no options left.
          //TODO Schrägbewegung wäre noch möglich !!
        }
      }
    }


    /// <summary>
    ///   This structure holds a movement option candidate (combination of direction and difference).
    /// </summary>
    private struct DirDiff : IComparable {
      public Direction Dir;
      public float Diff;

      public int CompareTo(Object obj) {
        var other = (DirDiff) obj;
        if (Diff < other.Diff) return -1;
        return Diff > other.Diff ? 1 : 0;
      }
    }

    public void Execute() {
      throw new NotImplementedException();
    }
  }
}
