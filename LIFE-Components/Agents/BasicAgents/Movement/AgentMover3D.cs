using System.Collections.Generic;
using System.Linq;
using LIFE.Components.Agents.BasicAgents.Environment;
using LIFE.Components.Agents.BasicAgents.Perception;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;
using MovementResult = LIFE.Components.Agents.BasicAgents.Perception.MovementResult;

namespace LIFE.Components.Agents.BasicAgents.Movement
{
    /// <summary>
    ///   AgentReference mover for three-dimensional cartesian environments.
    /// </summary>
    public class AgentMover3D : AgentMover
    {
        private readonly IESC _env; // The environment implementation to use.
        private readonly CartesianPosition _pos; // Agent position class.


        /// <summary>
        ///   Create a new cartesian 3D mover.
        ///   This function is automatically invoked in the abstract agent!
        /// </summary>
        /// <param name="env">The environment to use (probably some crappy ESC).</param>
        /// <param name="agentPos">AgentReference position data structure.</param>
        /// <param name="sensorArray">The agent's sensor array (to provide movement feedback).</param>
        public AgentMover3D(IESC env, CartesianPosition agentPos, SensorArray sensorArray)
            : base(sensorArray)
        {
            _env = env;
            _pos = agentPos;
        }


        /// <summary>
        ///   Try to insert this agent into the environment at the given position.
        /// </summary>
        /// <param name="x">AgentReference start position (x-coordinate).</param>
        /// <param name="y">AgentReference start position (y-coordinate).</param>
        /// <param name="z">AgentReference start position (z-coordinate).</param>
        /// <returns>Success flag. If failed, the agent may not be moved!</returns>
        public bool InsertIntoEnvironment(double x, double y, double z)
        {
            _pos.Shape = new Cuboid(new Vector3(1, 1, 1), new Vector3(x, y, z));
            return _env.Add(_pos, _pos.Shape.Position);
        }


        /// <summary>
        ///   MoveToPosition the agent forward with a given speed.
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        /// <returns>Interaction expressing this movement.</returns>
        public MovementAction MoveForward(double distance)
        {
            return MoveInDirection(distance, _pos.Yaw, _pos.Pitch);
        }


        /// <summary>
        ///   MoveToPosition the agent into a direction.
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        /// <param name="yaw">New agent heading.</param>
        /// <param name="pitch">AgentReference climb angle.</param>
        /// <returns>An interaction describing the movement.</returns>
        public MovementAction MoveInDirection(double distance, double yaw, double pitch)
        {
            var dir = new Direction();
            dir.SetYaw(yaw);
            dir.SetPitch(pitch);
            var vec = dir.GetDirectionalVector() * distance;
            return new MovementAction(() =>
            {
                var result = _env.Move(_pos, vec, dir);
                var success = MovementStatus.Success;
                var colObj = new List<CollisionObject>();
                if (result.Collisions != null && result.Collisions.Any())
                {
                    success = MovementStatus.SuccessCollision;
                    foreach (var entity in result.Collisions)
                    {
                        colObj.Add(new CollisionObject
                        {
                            AgentId = entity.AgentGuid.ToString(),
                            AgentType = entity.AgentType.Name
                        });
                        if (entity.CollisionType.Equals(CollisionType.MassiveAgent))
                        {
                            success = MovementStatus.FailedCollision;
                        }
                    }
                }
                MovementSensor.SetMovementResult(new MovementResult(success, colObj));
            });
        }


        /// <summary>
        ///   MoveToPosition the agent to a position.
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        /// <param name="x">X-coordinate target position.</param>
        /// <param name="y">Y-coordinate of target position.</param>
        /// <param name="z">Z-coordinate of target position.</param>
        /// <returns>An interaction describing the movement.</returns>
        public MovementAction MoveTowardsPosition(double distance, double x, double y, double z)
        {
            var diff = new Vector3(x - _pos.X, y - _pos.Y, z - _pos.Z);
            var dir = new Direction();
            dir.SetDirectionalVector(diff);
            var dist = new Vector3(x, y, z).GetDistance(diff); //| Check the traveling distance.
            if (distance > dist) distance = dist; //| If we would go too far, reduce it accordingly.
            return MoveInDirection(distance, dir.Yaw, dir.Pitch);
        }


        /// <summary>
        ///   Set the agent to a new position.
        /// </summary>
        /// <param name="x">X-coordinate target position.</param>
        /// <param name="y">Y-coordinate of target position.</param>
        /// <param name="z">Z-coordinate of target position.</param>
        /// <returns>An interaction describing the movement.</returns>
        public MovementAction SetToPosition(double x, double y, double z)
        {
            return MoveTowardsPosition(double.MaxValue, x, y, z);
        }
    }
}