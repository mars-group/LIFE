using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalskiAgent.Interactions;
using DalskiAgent.Movement;
using DalskiAgent.Perception;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Environment;
using Guid = System.Guid;

namespace DalskiAgent.newAsycClasses
{
    public class ContinuousMoverAsync
    {
        private readonly IAsyncEnvironment _env;              // Environment interaction interface.
        private readonly Guid _agentId;       // Spatial data, needed for some calculations.
        private readonly MovementDelegate _movementDelegate; // Delegate for Movementresults


        /// <summary>
        ///   Create a continuous agent mover.
        /// </summary>
        /// <param name="env">Environment interaction interface.</param>
        /// <param name="agentId">Spatial data, needed for some calculations.</param>
        /// <param name="movementSensor">Simple default sensor for movement feedback.</param>
        public ContinuousMoverAsync(IAsyncEnvironment env, Guid agentId, MovementDelegate movementDelegate)
        {
            _agentId = agentId;
            _env = env;
            _movementDelegate = movementDelegate;
        }


        /// <summary>
        ///   Moves the agent. This version uses turning speeds.
        /// </summary>
        /// <param name="speed">The movement speed.</param>
        /// <param name="pitchAs">Pitch changing angular speed.</param>
        /// <param name="yawAs">Rotary speed (vertical axis).</param>
        /// <returns>An interaction object that contains the code to execute this movement.</returns>  
        public IInteraction Move(double speed, float pitchAs, float yawAs)
        {
            var dir = new Direction();
            var tmp = _env.GetSpatialEntity(_agentId);
            dir.SetPitch(tmp.Shape.Rotation.Pitch + pitchAs);
            dir.SetYaw(tmp.Shape.Rotation.Yaw + yawAs);
            return Move(speed, dir);
        }


        /// <summary>
        ///   Moves the agent with some speed in a given direction. 
        /// </summary>
        /// <param name="speed">Movement speed.</param>
        /// <param name="dir">The direction [default: Use old heading].</param>
        /// <returns>An interaction object that contains the code to execute this movement.</returns> 
        public IInteraction Move(double speed, Direction dir = null)
        {
            if (dir == null) dir = _env.GetSpatialEntity(_agentId).Shape.Rotation;

            // Calculate target position based on current position, heading and speed.     
            var pitchRad = Direction.DegToRad(dir.Pitch);
            var yawRad = Direction.DegToRad(dir.Yaw);
            var factor = speed;
            var x = factor * Math.Cos(pitchRad) * Math.Sin(yawRad);
            var y = factor * Math.Sin(pitchRad);
            var z = factor * Math.Cos(pitchRad) * Math.Cos(yawRad);
            var vector = new Vector3(x, y, z);

            return new MovementAction(delegate {
                _env.Move(_agentId, vector, dir,_movementDelegate);
            });
        }


        /// <summary>
        ///   This function automatically sets the reference, speed, yaw and pitch 
        ///   values to go to the supplied point. It then returns an appropriate action. 
        /// </summary>
        /// <param name="speed">The agent's movement speed.</param>
        /// <param name="targetPos">A point the agent shall go to.</param>
        /// <returns>A movement action, ready for execution. </returns>
        public IInteraction MoveTowardsPosition(double speed, Vector3 targetPos) {

            var actShape = _env.GetSpatialEntity(_agentId).Shape;
            // Check, if we are already there. Otherwise no need to move anyway.
            var distance = actShape.Position.GetDistance(targetPos);
            if (Math.Abs(distance) <= float.Epsilon) return null;

            // Get the right direction.
            var diff = new Vector3(targetPos.X - actShape.Position.X,
                                   targetPos.Y - actShape.Position.Y,
                                   targetPos.Z - actShape.Position.Z);
            var dir = new Direction();
            dir.SetDirectionalVector(diff);

            // Check the speed. If we would go too far, reduce it accordingly.
            if (distance < speed) speed = distance;

            // Save calculated values to new movement class and return.
            return Move(speed, dir);
        }
    }
}
