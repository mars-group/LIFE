using System.Collections.Generic;

namespace LIFE.Components.Agents.BasicAgents.Perception
{
    /// <summary>
    ///   Result for a movement action.
    /// </summary>
    public class MovementResult
    {
        public readonly MovementStatus Status; // Movement success status.
        public readonly List<CollisionObject> CollisionObjects; // List of collision objects.


        /// <summary>
        ///   Create a new movement result.
        /// </summary>
        /// <param name="status">Success status of this movement.</param>
        /// <param name="colObj">List of collision objects (optional).</param>
        public MovementResult(MovementStatus status, List<CollisionObject> colObj = null)
        {
            Status = status;
            if (CollisionObjects == null) CollisionObjects = new List<CollisionObject>();
            else CollisionObjects = colObj;
        }
    }


    /// <summary>
    ///   Movement result statuses.
    /// </summary>
    public enum MovementStatus
    {
        Success,
        SuccessCollision,
        FailedCollision,
        OutOfBounds
    }


    /// <summary>
    ///   Collision entity information.
    /// </summary>
    public struct CollisionObject
    {
        public string AgentId; // Identifier of the agent.
        public string AgentType; // Type name of the agent.
    }
}