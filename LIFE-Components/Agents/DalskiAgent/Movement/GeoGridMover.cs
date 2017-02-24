using System;
using LIFE.Components.Agents.DalskiAgent.Agents;
using LIFE.Components.Agents.DalskiAgent.Interactions;
using LIFE.Components.Environments.GeoGridEnvironment;

namespace LIFE.Components.Agents.DalskiAgent.Movement {

  /// <summary>
  ///   Movement module for the GPS grid environment.
  /// </summary>
  public class GeoGridMover {

    private readonly IGeoGridEnvironment<GpsAgent> _geoGrid; // The grid environment to use.
    private readonly GpsAgent _gpsAgent;                     // Reference to this agent for orientation access.


    /// <summary>
    ///   Create a new grid mover.
    /// </summary>
    /// <param name="geoGrid">The grid environment to use.</param>
    /// <param name="gpsAgent">Reference to this agent for orientation access.</param>
    public GeoGridMover(IGeoGridEnvironment<GpsAgent> geoGrid, GpsAgent gpsAgent) {
      _geoGrid = geoGrid;
      _gpsAgent = gpsAgent;
    }


    /// <summary>
    ///   Moves the agent to a target.
    /// </summary>
    /// <param name="speed">Agent movement speed.</param>
    /// <param name="bearing">Bearing for movement. If unset, the old bearing is used.</param>
    /// <returns></returns>
    public IInteraction Move(double speed, double bearing = -1) {
      if (bearing.Equals(-1)) {
        bearing = _gpsAgent.Bearing;
      }

      // Calculate target position based on current position, heading and speed.
      double targetLat, targetLong;
      var distance = speed;
      CalculateNewCoordinates(_gpsAgent.Latitude, _gpsAgent.Longitude, bearing, distance, out targetLat, out targetLong);
      return new MovementAction(() => {
        try {
          var updatedAgent = _geoGrid.Move(_gpsAgent, targetLat, targetLong);
          _gpsAgent.Bearing = updatedAgent.Bearing;
          _gpsAgent.Latitude = updatedAgent.Latitude;
          _gpsAgent.Longitude = updatedAgent.Longitude;
        }
        catch (IndexOutOfRangeException) { /* Ignore range exception. */ }
      });
    }


    /// <summary>
    ///   Generates a movement action to the GPS destination.
    /// </summary>
    /// <param name="speed">Movement speed</param>
    /// <param name="targetLat">Latitude of target position.</param>
    /// <param name="targetLong">Longitude of target position.</param>
    /// <returns>The movement interaction.</returns>
    public IInteraction MoveToTarget(double speed, double targetLat, double targetLong) {
      return Move(speed, CalculateBearingToCoordinate(targetLat, targetLong));
    }


    /// <summary>
    ///   Calculates a new geocoordinate based on a current position and a directed movement.
    /// </summary>
    /// <param name="originLat">Origin latitude [in degree].</param>
    /// <param name="originLong">Origin longitude [in degree].</param>
    /// <param name="bearing">The bearing (compass heading, 0 - lt.360°) [in degree].</param>
    /// <param name="distance">The travelling distance [in m].</param>
    /// <param name="lat2">Output of destination latitude.</param>
    /// <param name="long2">Output of destination longitude.</param>
    private static void CalculateNewCoordinates(double originLat, double originLong, double bearing, double distance,
      out double lat2, out double long2) {
      const double deg2Rad = 0.0174532925; // Degree to radians conversion.
      const double rad2Deg = 57.2957795;   // Radians to degree factor.
      const double radius = 6371;          // Radius of the Earth.

      // Distance is needed in kilometers, angles in radians.
      distance /= 1000;
      bearing *= deg2Rad;
      originLat *= deg2Rad;
      originLong *= deg2Rad;

      // Perform calculation of new coordinate.
      var dr = distance/radius;
      lat2 = Math.Asin(Math.Sin(originLat)*Math.Cos(dr) +
                       Math.Cos(originLat)*Math.Sin(dr)*Math.Cos(bearing));
      long2 = originLong + Math.Atan2(Math.Sin(bearing)*Math.Sin(dr)*Math.Cos(originLat),
        Math.Cos(dr) - Math.Sin(originLat)*Math.Sin(lat2));

      // Convert results back to degrees.
      lat2 *= rad2Deg;
      long2 *= rad2Deg;
    }


    /// <summary>
    ///   Calculate the bearing to a GPS coordinate.
    /// </summary>
    /// <param name="targetLat">Latitude (+: ↑ north | -: ↓ south) of the target coordinate.</param>
    /// <param name="targetLong">Longitude (-: ← west | +: → east) of the target coordinate.</param>
    /// <returns>A direction with the calculated bearing as yaw and a pitch of 0°.</returns>
    private double CalculateBearingToCoordinate(double targetLat, double targetLong) {
      var diffLat = targetLat - _gpsAgent.Latitude;   // Latitude difference.   +: ↑ (north)| -: ↓ (south)
      var diffLng = targetLong - _gpsAgent.Longitude; // Longitude difference.  +: → (east) | -: ← (west)

      // If there's no latitude difference, the tangens would fail (division by zero).  
      if (Math.Abs(diffLat) <= double.Epsilon) {
        if (diffLng > 0) return 90;
        return 270;
      }

      // Otherwise perform triangulation.
      var bearing = Math.Atan(diffLng/diffLat)*57.295779;
      if (diffLat < 0) bearing += 180; // Range  90° to 270° correction.
      if (bearing < 0) bearing += 360; // Range 270° to 360° correction.
      return bearing;
    }
  }
}