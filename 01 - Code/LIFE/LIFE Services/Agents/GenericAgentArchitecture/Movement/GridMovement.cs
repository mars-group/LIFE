using CommonTypes.DataTypes;
using ESCTestLayer.Interface;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   Direction enumeration for grid movement.
  /// </summary>
  public enum Direction { Left, Right, Up, Down, UpLeft, UpRight, DownLeft, DownRight }


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

      //...
      do {

        //TODO Copy awesome shit here...
      
        Vector2f tgt = new Vector2f(0, 0);
        
        // Check, if the agent has enough movement points.  
        float distance = Position.GetDistance(tgt);
        if (distance <= movementPoints) {

          //TODO HALT fehlerhaft: Abziehen erst nach Erfolg.
          if (_failureCostEnabled) movementPoints -= distance;

        }
      }
      while (movementPoints >= 1);


      //TODO Optimierung möglich und sinnvoll: Wenn z.B. schräg erste Wahl wäre, aber MP=1:
      // Lieber geradeaus in passende Richtung gehen, als gar nicht bewegen! 

    }
  }
}
