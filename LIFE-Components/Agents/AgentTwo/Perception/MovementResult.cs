
using System.Collections.Generic;

namespace LIFE.Components.Agents.AgentTwo.Perception {

  public class MovementResult {

    public readonly MovementStatus Status;
    public readonly List<CollisionObject> CollisionObjects;

    public MovementResult(MovementStatus status, List<CollisionObject> colObj = null) {
      Status = status;  
      if (CollisionObjects == null) CollisionObjects = new List<CollisionObject>();
      else CollisionObjects = colObj;
    }

  }


  public enum MovementStatus { Success, SuccessCollision, FailedCollision, OutOfBounds }

  public struct CollisionObject {
    public string AgentId;
    public string AgentType;
  }
}