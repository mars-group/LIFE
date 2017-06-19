using System;
using AsyncAgents.Perception;
using EnvironmentServiceComponent.SpatialAPI.Environment;
using LIFE.Components.Agents.BasicAgents.Movement;
using LIFE.Components.Agents.BasicAgents.Perception;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace AsyncAgents.Movement
{

    public class AgentMover3DAsync : AgentMover
    {
        private readonly ISpatialEntity _initEntity;  // Spatial data, needed for some calculations.    

        private readonly Guid _agentId;  // Spatial data, needed for some calculations.    
        private readonly MovementDelegate _movementDelegate;
        private IAsyncEnvironment _env;

        /// <summary>
        ///   Instantiate a new agent mover.
        /// </summary>
        /// <param name="env">IEnvironment implementation to use.</param>
        /// <param name="agentId">Spatial data, needed for some calculations.</param>
        /// <param name="sensorArray">Sensor and perception storage (used for movement results).</param>
        public AgentMover3DAsync(IAsyncEnvironment env, ISpatialEntity entity, SensorArray sensorArray): base(sensorArray)
        {
            var movementSensor = new MovementSensorAsync();
            sensorArray.AddSensor(movementSensor);
            _movementDelegate = movementSensor.GetDelegate();
            _agentId = entity.AgentGuid;
            _initEntity = entity;
            _env = env;
//            Direct = new DirectMoverAsync(env, agentId, _movementDelegate);
//            Continuous = new ContinuousMoverAsync(env, agentId, _movementDelegate);
//            Grid = new GridMoverAsync(env, agentId, _movementDelegate);
//            GPS = new GPSMoverAsync(env, agentId, _movementDelegate);
        }


        /// <summary>
        ///   Try to insert this agent into the environment at the given position.
        /// </summary>
        /// <param name="x">AgentReference start position (x-coordinate).</param>
        /// <param name="y">AgentReference start position (y-coordinate).</param>
        /// <param name="z">AgentReference start position (z-coordinate).</param>
        /// <param name="shape">Shape of the agent.</param>
        /// <returns>Success flag. If failed, the agent may not be moved!</returns>
        public void InsertIntoEnvironment(double x, double y, double z, IShape shape = null)
        {
            // Set agent shape. If the agent has no shape yet, create a cube facing north and add at a random position. 
            if (shape != null) _initEntity.Shape = shape;
            else _initEntity.Shape = new Cuboid(new Vector3(1.0, 1.0, 1.0), new Vector3(), new Direction());
            _env.Add(_initEntity, _movementDelegate);
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
        /// <param name="distance">The movement speed.</param>
        /// <param name="pitchAs">Pitch changing angular speed.</param>
        /// <param name="yawAs">Rotary speed (vertical axis).</param>
        /// <returns>An interaction object that contains the code to execute this movement.</returns>  
        public MovementAction MoveInDirection(double distance, double yawAs, double pitchAs)
        {
            var dir = _env.GetSpatialEntity(_agentId).Shape.Rotation;
            dir.SetYaw(yawAs);
            dir.SetPitch(pitchAs);
            // Calculate target position based on current position, heading and speed.     
            var pitchRad = Direction.DegToRad(dir.Pitch);
            var yawRad = Direction.DegToRad(dir.Yaw);
            var factor = distance;
            var x = factor * Math.Cos(pitchRad) * Math.Sin(yawRad);
            var y = factor * Math.Sin(pitchRad);
            var z = factor * Math.Cos(pitchRad) * Math.Cos(yawRad);
            var vector = new Vector3(x, y, z);

            return new MovementAction(delegate {
                _env.Move(_agentId, vector, dir, _movementDelegate);
            });
        }

        /// <summary>
        ///   MoveToPosition the agent forward with a given speed.
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        /// <returns>Interaction expressing this movement.</returns>
        public MovementAction MoveForward(double distance)
        {
            var entity = _env.GetSpatialEntity(_agentId);
            return MoveInDirection(distance, entity.Shape.Rotation.Yaw, entity.Shape.Rotation.Pitch);
        }



        //        /// <summary>
        //        ///   Moves the agent with some speed in a given direction. 
        //        /// </summary>
        //        /// <param name="distance">Movement speed.</param>
        //        /// <param name="dir">The direction [default: Use old heading].</param>
        //        /// <returns>An interaction object that contains the code to execute this movement.</returns> 
        //        public MovementAction Move(double distance, Direction dir = null)
        //        {
        //            if (dir == null) dir = _env.GetSpatialEntity(_agentId).Shape.Rotation;
        //
        //            // Calculate target position based on current position, heading and speed.     
        //            var pitchRad = Direction.DegToRad(dir.Pitch);
        //            var yawRad = Direction.DegToRad(dir.Yaw);
        //            var factor = distance;
        //            var x = factor * Math.Cos(pitchRad) * Math.Sin(yawRad);
        //            var y = factor * Math.Sin(pitchRad);
        //            var z = factor * Math.Cos(pitchRad) * Math.Cos(yawRad);
        //            var vector = new Vector3(x, y, z);
        //
        //            return new MovementAction(delegate {
        //                _env.Move(_agentId, vector, dir, _movementDelegate);
        //            });
        //        }


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

            var actShape = _env.GetSpatialEntity(_agentId).Shape;
            // Check, if we are already there. Otherwise no need to move anyway.
            var distanceToPos = actShape.Position.GetDistance(new Vector3(x,y,z));
            if (Math.Abs(distanceToPos) <= float.Epsilon) return null;

            // Get the right direction.
            var diff = new Vector3(x - actShape.Position.X,
                y - actShape.Position.Y,
                z - actShape.Position.Z);
            var dir = new Direction();
            dir.SetDirectionalVector(diff);

            // Check the speed. If we would go too far, reduce it accordingly.
            if (distanceToPos < distance) distance = distanceToPos;

            // Save calculated values to new movement class and return.
            return MoveInDirection(distance, dir.Yaw,dir.Pitch);
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
