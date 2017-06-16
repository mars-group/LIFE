using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIFE.Components.Agents.BasicAgents.Movement;
using LIFE.Components.Agents.BasicAgents.Perception;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;

namespace DalskiAgent.newAsycClasses
{

    public class AgentMoverAsync
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
        public AgentMoverAsync(IAsyncEnvironment env, Guid agentId, SensorArray sensorArray)
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
    }
}
