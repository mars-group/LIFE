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
    public class GPSMoverAsync
    {

        private readonly IAsyncEnvironment _env;              // Environment interaction interface.
        private readonly Guid _agentId;       // Spatial data, needed for some calculations.
        private readonly MovementDelegate _movementDelegate; // Delegate for movement result


        /// <summary>
        ///   Create a GPS-based agent mover.
        /// </summary>
        /// <param name="env">Environment interaction interface.</param>
        /// <param name="agentId">Spatial data, needed for some calculations.</param>
        /// <param name="movementSensor">Simple default sensor for movement feedback.</param>
        public GPSMoverAsync(IAsyncEnvironment env, Guid agentId, MovementDelegate movementDelegate)
        {
            _agentId = agentId;
            _movementDelegate = movementDelegate;
            _env = env;
        }



        /// <summary>
        ///   Moves the agent with some speed in a given direction. 
        /// </summary>
        /// <param name="speed">Movement speed.</param>
        /// <param name="dir">The direction [default: Use old heading].</param>
        /// <returns>An interaction object that contains the code to execute this movement.</returns> 
        public IInteraction Move(double speed, Direction dir = null) {
            var tmpShape = _env.GetSpatialEntity(_agentId).Shape;
            if (dir == null) dir = tmpShape.Rotation;

            // Calculate target position based on current position, heading and speed.     
            double lat, lng;
            double distance = speed;
            CalculateNewCoordinates(tmpShape.Position.X, tmpShape.Position.Z, dir.Yaw, distance, out lat, out lng);
            var vector = new Vector3(lat - tmpShape.Position.X, lng - tmpShape.Position.Z, tmpShape.Position.Y);
            return new MovementAction(delegate {
                _env.Move(_agentId, vector, dir, _movementDelegate);
            });
        }


        /// <summary>
        ///   Calculates a new geocoordinate based on a current position and a directed movement. 
        /// </summary>
        /// <param name="lat1">Origin latitude [in degree].</param>
        /// <param name="long1">Origin longitude [in degree].</param>
        /// <param name="bearing">The bearing (compass heading, 0 - lt.360°) [in degree].</param>
        /// <param name="distance">The travelling distance [in m].</param>
        /// <param name="lat2">Output of destination latitude.</param>
        /// <param name="long2">Output of destination longitude.</param>
        private static void CalculateNewCoordinates(double lat1, double long1, double bearing, double distance, out double lat2, out double long2)
        {

            const double deg2Rad = 0.0174532925; // Degree to radians conversion.
            const double rad2Deg = 57.2957795;   // Radians to degree factor.
            const double radius = 6371;          // Radius of the Earth.

            // Distance is needed in kilometers, angles in radians.
            distance /= 1000;
            bearing *= deg2Rad;
            lat1 *= deg2Rad;
            long1 *= deg2Rad;

            // Perform calculation of new coordinate.
            double dr = distance / radius;
            lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dr) +
                   Math.Cos(lat1) * Math.Sin(dr) * Math.Cos(bearing));
            long2 = long1 + Math.Atan2(Math.Sin(bearing) * Math.Sin(dr) * Math.Cos(lat1),
                            Math.Cos(dr) - Math.Sin(lat1) * Math.Sin(lat2));

            // Convert results back to degrees.
            lat2 *= rad2Deg;
            long2 *= rad2Deg;
        }

    }
}
