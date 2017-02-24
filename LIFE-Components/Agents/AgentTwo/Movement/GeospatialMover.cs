using System;
using LIFE.Components.Agents.AgentTwo.Environment;
using LIFE.Components.Agents.AgentTwo.Perception;
using LIFE.Components.Agents.AgentTwo.Reasoning;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;
using LIFE.Components.Environments.GeoGridEnvironment;

namespace LIFE.Components.Agents.AgentTwo.Movement {

  /// <summary>
  ///   Movement module for geospatial agents.
  /// </summary>
  public class GeospatialMover : AgentMover {

    private readonly IGeoGridEnvironment<GeoPosition> _geoGrid; // The grid environment to use.
    private readonly GeoPosition _position;                     // Agent position structure.


    /// <summary>
    ///   Create a new geospatial mover.
    ///   This function is automatically invoked in the abstract agent!
    /// </summary>
    /// <param name="env">The geospatial environment to use.</param>
    /// <param name="agentPos">Agent position data structure.</param>
    /// <param name="sensorArray">The agent's sensor array (to provide movement feedback).</param>
    public GeospatialMover(IGeoGridEnvironment<GeoPosition> env, GeoPosition agentPos, SensorArray sensorArray)
      : base(sensorArray) {
      _geoGrid = env;
      _position = agentPos;
      env.Insert(_position);
    }


    /// <summary>
    ///   MoveToPosition the agent forward with a given speed.
    /// </summary>
    /// <param name="distance">The distance to move.</param>
    /// <returns>Interaction expressing this movement.</returns>
    public MovementAction MoveForward(double distance) {
      return MoveInDirection(distance, _position.Bearing);
    }


    /// <summary>
    ///   MoveToPosition the agent into a direction.
    /// </summary>
    /// <param name="distance">The distance to move.</param>
    /// <param name="bearing">New agent bearing.</param>
    /// <returns>An interaction describing the movement.</returns>
    public MovementAction MoveInDirection(double distance, double bearing) {
      double targetLat, targetLong;
      CalculateNewCoordinates(
        _position.Latitude, _position.Longitude, // At this position are we now.
        bearing, distance,                       // This is our heading and traveling distance.
        out targetLat, out targetLong            // Return the calculated target position.
      );
      return SetToPosition(targetLat, targetLong);
    }


    /// <summary>
    ///   MoveToPosition the agent to a position.
    /// </summary>
    /// <param name="distance">The distance to move.</param>
    /// <param name="targetLat">Latitude of target position.</param>
    /// <param name="targetLng">Longitude of target position.</param>
    /// <returns>An interaction describing the movement.</returns>
    public MovementAction MoveTowardsPosition(double distance, double targetLat, double targetLng) {
      var bearing = CalculateBearing(_position.Latitude, _position.Longitude, targetLat, targetLng);
      return MoveInDirection(distance, bearing);
    }


    /// <summary>
    ///   Set the agent to a new position.
    /// </summary>
    /// <param name="lat">Latitude of new position.</param>
    /// <param name="lng">Longitude of new position.</param>
    /// <returns>An interaction describing the movement.</returns>
    public MovementAction SetToPosition(double lat, double lng) {
      return new MovementAction(() => {
        GeoPosition newPos;
        try { newPos = _geoGrid.MoveToPosition(_position, lat, lng); }
        catch (IndexOutOfRangeException) { return; } // Movement failed, stay at the old position.
        _position.Bearing = newPos.Bearing;
        _position.Latitude = newPos.Latitude;
        _position.Longitude = newPos.Longitude;
        MovementSensor.SetMovementResult(new MovementResult());
      });
    }


    /// <summary>
    ///   Calculate the bearing to a GPS coordinate.
    /// </summary>
    /// <param name="lat">Latitude (+: ↑ north | -: ↓ south) of the target coordinate.</param>
    /// <param name="lng">Longitude (-: ← west | +: → east) of the target coordinate.</param>
    /// <returns>A direction with the calculated bearing as yaw and a pitch of 0°.</returns>
    public double CalculateBearingToCoordinate(double lat, double lng) {
      return CalculateBearing(_position.Latitude, _position.Longitude, lat, lng);
    }
  }
}