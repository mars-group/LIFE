﻿using System;
using LIFE.API.Environment.GeoCommon;
using LIFE.Components.Agents.BasicAgents.Agents;
using LIFE.Components.Agents.BasicAgents.Perception;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.Environments.GeoGridEnvironment;

namespace LIFE.Components.Agents.BasicAgents.Movement {

  /// <summary>
  ///   Movement module for geospatial agents.
  /// </summary>
  public class GeoMover<T> : AgentMover where T : IGeoCoordinate {

    private readonly IGeoGridEnvironment<IGeoCoordinate> _geoGrid; // The grid environment to use.
    private readonly GeoAgent<T> _agent;                        // AgentReference position structure.


    /// <summary>
    ///   Create a new geospatial mover.
    ///   This function is automatically invoked in the abstract agent!
    /// </summary>
    /// <param name="env">The geospatial environment to use.</param>
    /// <param name="pos">AgentReference position data structure.</param>
    /// <param name="sensorArray">The agent's sensor array (to provide movement feedback).</param>
    public GeoMover(IGeoGridEnvironment<IGeoCoordinate> env, GeoAgent<T> agent, SensorArray sensorArray)
      : base(sensorArray) {
      _geoGrid = env;
      _agent = agent;
    }


    /// <summary>
    ///   Try to insert this agent into the environment at the given position.
    /// </summary>
    /// <param name="lat">AgentReference start position (latitude).</param>
    /// <param name="lon">AgentReference start position (longitude).</param>
    public void InsertIntoEnvironment(double lat, double lon) {
      _agent.SetPosition(new GeoPosition(lat, lon));
      _geoGrid.Insert(_agent);
    }


    /// <summary>
    ///   MoveToPosition the agent forward with a given speed.
    /// </summary>
    /// <param name="distance">The distance to move.</param>
    /// <returns>Interaction expressing this movement.</returns>
    public MovementAction MoveForward(double distance) {
      return MoveInDirection(distance, _agent.Bearing);
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
        _agent.Latitude, _agent.Longitude, // At this position are we now.
        bearing, distance,                       // This is our heading and traveling distance.
        out targetLat, out targetLong            // Return the calculated target position.
      );
      return SetToPosition(targetLat, targetLong, bearing);
    }


    /// <summary>
    ///   MoveToPosition the agent to a position.
    /// </summary>
    /// <param name="distance">The distance to move.</param>
    /// <param name="targetLat">Latitude of target position.</param>
    /// <param name="targetLng">Longitude of target position.</param>
    /// <returns>An interaction describing the movement.</returns>
    public MovementAction MoveTowardsPosition(double distance, double targetLat, double targetLng) {
      var bearing = CalculateBearing(_agent.Latitude, _agent.Longitude, targetLat, targetLng);
      return MoveInDirection(distance, bearing);
    }


    /// <summary>
    ///   Set the agent to a new position.
    /// </summary>
    /// <param name="lat">Latitude of new position.</param>
    /// <param name="lng">Longitude of new position.</param>
    /// <param name="bearing">New agent bearing.</param>
    /// <returns>An interaction describing the movement.</returns>
    public MovementAction SetToPosition(double lat, double lng, double bearing) {
      return new MovementAction(() => {
        try {
          var newPos = _geoGrid.MoveToPosition(new GeoCoordinate(_agent.Latitude, _agent.Longitude), lat, lng);
          _agent.SetPosition(new GeoPosition(newPos.Latitude, newPos.Longitude), bearing);
          MovementSensor.SetMovementResult(new MovementResult(MovementStatus.Success));
        }
        catch (IndexOutOfRangeException) { // Movement failed, stay at the old position.
          MovementSensor.SetMovementResult(new MovementResult(MovementStatus.FailedCollision));
        } 
      });
    }


    /// <summary>
    ///   Calculate the bearing to a GPS coordinate.
    /// </summary>
    /// <param name="lat">Latitude (+: ↑ north | -: ↓ south) of the target coordinate.</param>
    /// <param name="lng">Longitude (-: ← west | +: → east) of the target coordinate.</param>
    /// <returns>A direction with the calculated bearing as yaw and a pitch of 0°.</returns>
    public double CalculateBearingToCoordinate(double lat, double lng) {
      return CalculateBearing(_agent.Latitude, _agent.Longitude, lat, lng);
    }
  }
}