using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIFE.Components.Agents.BasicAgents.Movement;
using LIFE.Components.Agents.BasicAgents.Perception;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;

namespace DalskiAgent.newAsycClasses
{

    public class AgentMover3DAsync
    {

        private readonly Guid _agentId;  // Spatial data, needed for some calculations.    
        private readonly MovementDelegate _movementDelegate;
        private IAsyncEnvironment _env;
        /// <summary>
        ///   Module for grid-style movement.
        /// </summary>
        public readonly GridMoverAsync Grid;

        /// <summary>
        ///   Movement module with speeds and position calculation.
        /// </summary>
        public readonly ContinuousMoverAsync Continuous;

        /// <summary>
        ///   Enables basic agent movement by direct placement.
        /// </summary>
        public readonly DirectMoverAsync Direct;

        /// <summary>
        ///   Movement module for applications with GPS coordinates.
        /// </summary>
        public readonly GPSMoverAsync GPS;


        /// <summary>
        ///   Instantiate a new agent mover.
        /// </summary>
        /// <param name="env">IEnvironment implementation to use.</param>
        /// <param name="agentId">Spatial data, needed for some calculations.</param>
        /// <param name="sensorArray">Sensor and perception storage (used for movement results).</param>
        public AgentMover3DAsync(IAsyncEnvironment env, Guid agentId, SensorArray sensorArray)
        {
            var movementSensor = new MovementSensorAsync();
            sensorArray.AddSensor(movementSensor);
            _movementDelegate = movementSensor.GetDelegate();
            _agentId = agentId;
            _env = env;
            Direct = new DirectMoverAsync(env, agentId, _movementDelegate);
            Continuous = new ContinuousMoverAsync(env, agentId, _movementDelegate);
            Grid = new GridMoverAsync(env, agentId, _movementDelegate);
            GPS = new GPSMoverAsync(env, agentId, _movementDelegate);
        }


        /// <summary>
        ///   Calculate the needed direction towards a given position.
        /// </summary>
        /// <param name="target">The target to get orientation to.</param>
        /// <returns>The yaw (corrected to 0 - lt. 360). </returns>
        public Direction CalculateDirectionToTarget(Vector3 target) {
            var tmpSpatial = _env.GetSpatialEntity(_agentId); 
            var diff = new Vector3(target.X - tmpSpatial.Shape.Position.X,
                                   target.Y - tmpSpatial.Shape.Position.Y,
                                   target.Z - tmpSpatial.Shape.Position.Z);

            // Create new direction, set joint vector as reference and return.
            var dir = new Direction();
            dir.SetDirectionalVector(diff);
            return dir;
        }

        // <summary>
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
                _env.Move(_agentId, vector, dir, _movementDelegate);
            });
        }


        /// <summary>
        ///   This function automatically sets the reference, speed, yaw and pitch 
        ///   values to go to the supplied point. It then returns an appropriate action. 
        /// </summary>
        /// <param name="speed">The agent's movement speed.</param>
        /// <param name="targetPos">A point the agent shall go to.</param>
        /// <returns>A movement action, ready for execution. </returns>
        public IInteraction MoveTowardsPosition(double speed, Vector3 targetPos)
        {

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



        /// <summary>
        ///   Set the agent to a new position.
        /// </summary>
        /// <param name="position">The target position.</param>
        /// <param name="dir">The new direction [default: use old heading].</param> 
        /// <returns>An interaction object that contains the code to execute this movement.</returns>
        public IInteraction SetToPosition(Vector3 position, Direction dir = null)
        {
            var tmpShape = _env.GetSpatialEntity(_agentId).Shape;
            if (dir == null) dir = tmpShape.Rotation;
            var vector = new Vector3
            (
                position.X - tmpShape.Position.X,
                position.Y - tmpShape.Position.Y,
                position.Z - tmpShape.Position.Z
            );

            return new MovementAction
            (delegate {
                _env.Move(_agentId, vector, dir, _movementDelegate);
            });
        }
    }
}
