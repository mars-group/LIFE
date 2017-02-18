using System;
using LIFE.Components.DalskiAgent.Interactions;
using LIFE.Components.DalskiAgent.Perception;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;

namespace LIFE.Components.DalskiAgent.Movement {
  
  /// <summary>
  ///   Movement module for applications with GPS coordinates instead of a cartesian system.
  /// </summary>
  public class GPSMover {
    
    private readonly IEnvironment _env;              // Environment interaction interface.
    private readonly AgentEntity _spatialData;       // Spatial data, needed for some calculations.
    private readonly MovementSensor _movementSensor; // Simple default sensor for movement feedback.
    

    /// <summary>
    ///   Create a GPS-based agent mover.
    /// </summary>
    /// <param name="env">Environment interaction interface.</param>
    /// <param name="spatialData">Spatial data, needed for some calculations.</param>
    /// <param name="movementSensor">Simple default sensor for movement feedback.</param>
    public GPSMover(IEnvironment env, AgentEntity spatialData, MovementSensor movementSensor) {
      _spatialData = spatialData;
      _movementSensor = movementSensor;
      _env = env;
    }



    /// <summary>
    ///   Moves the agent with some speed in a given direction. 
    /// </summary>
    /// <param name="speed">Movement speed.</param>
    /// <param name="dir">The direction [default: Use old heading].</param>
    /// <returns>An interaction object that contains the code to execute this movement.</returns> 
    public IInteraction Move(double speed, Direction dir = null) {
      if (dir == null) dir = _spatialData.Direction;
      
      // Calculate target position based on current position, heading and speed.     
      double lat, lng;
      double distance = speed;
      CalculateNewCoordinates(_spatialData.Position.X, _spatialData.Position.Z, dir.Yaw, distance, out lat, out lng);
      var vector = new Vector3(lat-_spatialData.Position.X, _spatialData.Position.Y, lng-_spatialData.Position.Z);
      return new MovementAction(delegate {
        var result = _env.Move(_spatialData, vector, dir);
        _movementSensor.SetMovementResult(result);
      });
    }


    /// <summary>
    ///   Calculate the bearing to a GPS coordinate.
    /// </summary>
    /// <param name="lat">Latitude (+: ↑ north | -: ↓ south) of the target coordinate.</param>
    /// <param name="lng">Longitude (-: ← west | +: → east) of the target coordinate.</param>
    /// <returns>A direction with the calculated bearing as yaw and a pitch of 0°.</returns>
    public Direction CalculateBearingToCoordinate(double lat, double lng) {
      var bearing = CalculateBearing(_spatialData.Position.X, _spatialData.Position.Z, lat, lng);
      var direction = new Direction();
      direction.SetYaw(bearing);
      return direction;
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
    private static void CalculateNewCoordinates(double lat1, double long1, double bearing, double distance, out double lat2, out double long2) {

      const double deg2Rad = 0.0174532925; // Degree to radians conversion.
      const double rad2Deg = 57.2957795;   // Radians to degree factor.
      const double radius = 6371;          // Radius of the Earth.
          
      // Distance is needed in kilometers, angles in radians.
      distance /= 1000;
      bearing *= deg2Rad;
      lat1    *= deg2Rad;
      long1   *= deg2Rad;

      // Perform calculation of new coordinate.
      double dr = distance/radius;
      lat2 = Math.Asin(Math.Sin(lat1)*Math.Cos(dr) + 
             Math.Cos(lat1)*Math.Sin(dr)*Math.Cos(bearing));
      long2 = long1 + Math.Atan2(Math.Sin(bearing)*Math.Sin(dr)*Math.Cos(lat1),
                      Math.Cos(dr) - Math.Sin(lat1)*Math.Sin(lat2));

      // Convert results back to degrees.
      lat2  *= rad2Deg;
      long2 *= rad2Deg;
    }


    /// <summary>
    ///   Calculate the bearing between two GPS coordinates.
    /// </summary>
    /// <param name="lat1">Latitude of the origin.</param>
    /// <param name="lng1">Longitude of the origin.</param>
    /// <param name="lat2">Latitude of the destination.</param>
    /// <param name="lng2">Longitude of the destination.</param>
    /// <returns>The bearing (compass heading) towards the target.</returns>
    private static double CalculateBearing(double lat1, double lng1, double lat2, double lng2) {    
      var diffLat = lat2 - lat1; // Latitude difference.   +: ↑ (north)| -: ↓ (south)
      var diffLng = lng2 - lng1; // Longitude difference.  +: → (east) | -: ← (west)

      /*      ↑         MATH REMINDER
       *      | GK (lng) 
       *      +======+      Trigonometry Rules
       *      |     /       *******************
       *      |    /        sin(α) = GK / Hypo 
       *  AK  |   /Hypo     cos(α) = AK / Hypo 
       *(lat.)|  /	        tan(α) = GK / AK 
       *      |α/           \ To get the angle, use the
       *      |/             \inverse function (arcsin etc.)
       *      +------------------------------>   */
             

      // If there's no latitude difference, the tangens would fail (division by zero).  
      if (Math.Abs(diffLat) <= double.Epsilon) {
        if (diffLng > 0) return 90;
        return 270;
      }

      // Otherwise perform triangulation.
      double yaw = Math.Atan(diffLng/diffLat) * 57.295779;
      if (diffLat < 0) yaw += 180;  // Range  90° to 270° correction. 
      if (yaw < 0)     yaw += 360;  // Range 270° to 360° correction.    
      return yaw;
    }
  }
}