using System;
using LIFE.Components.Agents.DalskiAgent.Perception;

namespace LIFE.Components.Agents.DalskiAgent.Movement {

  /// <summary>
  ///   Base class for the agent movement modules.
  /// </summary>
  public abstract class AgentMover {

    private const double Deg2Rad =  0.0174532925;     // Degree to radians conversion.
    private const double Rad2Deg = 57.2957795131;     // Radians to degree factor.
    private const double EarthRadius = 6371;          // Radius of the Earth.
    protected readonly MovementSensor MovementSensor; // Sensor to provide movement feedback.


    /// <summary>
    ///   Create a new base agent mover.
    /// </summary>
    /// <param name="sensorArray">Sensor to provide movement feedback.</param>
    protected AgentMover(SensorArray sensorArray) {
      MovementSensor = new MovementSensor();
      sensorArray.AddSensor(MovementSensor);
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
    protected static void CalculateNewCoordinates(double originLat, double originLong, double bearing, double distance,
      out double lat2, out double long2) {

      // Distance is needed in kilometers, angles in radians.
      distance /= 1000;
      bearing *= Deg2Rad;
      originLat *= Deg2Rad;
      originLong *= Deg2Rad;

      // Perform calculation of new coordinate.
      var dr = distance/EarthRadius;
      lat2 = Math.Asin(Math.Sin(originLat)*Math.Cos(dr) +
                       Math.Cos(originLat)*Math.Sin(dr)*Math.Cos(bearing));
      long2 = originLong + Math.Atan2(Math.Sin(bearing)*Math.Sin(dr)*Math.Cos(originLat),
                           Math.Cos(dr) - Math.Sin(originLat)*Math.Sin(lat2));

      // Convert results back to degrees.
      lat2 *= Rad2Deg;
      long2 *= Rad2Deg;
    }


    /// <summary>
    ///   Calculate the bearing between two GPS coordinates.
    /// </summary>
    /// <param name="lat1">Latitude of the origin.</param>
    /// <param name="lng1">Longitude of the origin.</param>
    /// <param name="lat2">Latitude of the destination.</param>
    /// <param name="lng2">Longitude of the destination.</param>
    /// <returns>The bearing (compass heading) towards the target.</returns>
    protected static double CalculateBearing(double lat1, double lng1, double lat2, double lng2) {

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
      var yaw = Math.Atan(diffLng/diffLat) * 57.295779;
      if (diffLat < 0) yaw += 180;  // Range  90° to 270° correction.
      if (yaw < 0)     yaw += 360;  // Range 270° to 360° correction.
      return yaw;
    }


    /// <summary>
    ///   Calculate a movement vector based on current position, headings and distance.
    /// </summary>
    /// <param name="pos">Current position.</param>
    /// <param name="distance">Traveling distance.</param>
    /// <param name="yaw">Heading of the agent.</param>
    /// <param name="pitch">Climb angle.</param>
    /// <returns>The movement vector [x,y,z].</returns>
    protected static double[] CalculateMovementVector(double[] pos, double distance, double yaw, double pitch) {
      var pitchRad = pitch * Deg2Rad;
      var yawRad = yaw * Deg2Rad;
      return new [] {
        distance * Math.Cos(pitchRad) * Math.Sin(yawRad),
        distance * Math.Cos(pitchRad) * Math.Cos(yawRad),
        distance * Math.Sin(pitchRad)
      };
    }


    /// <summary>
    ///   Calculate the distance between two points.
    /// </summary>
    /// <param name="x1">First position (x coordinate).</param>
    /// <param name="y1">First position (y coordinate).</param>
    /// <param name="x2">Second position (x coordinate).</param>
    /// <param name="y2">Second position (y coordinate).</param>
    /// <returns>The distance between these two points.</returns>
    public static double CalculateDistance2D(double x1, double y1, double x2, double y2) {
      var a = x2 - x1;
      var b = y2 - y1;
      var c2 = a * a + b * b;
      if (c2.Equals(0)) return 0;
      return Math.Sqrt(c2);
    }
  }
}