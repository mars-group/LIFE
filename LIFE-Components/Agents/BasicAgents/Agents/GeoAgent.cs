﻿using LIFE.API.GeoCommon;
using LIFE.API.Layer;
using LIFE.API.Results;
using LIFE.Components.Agents.BasicAgents.Movement;
using LIFE.Components.Environments.GeoGridEnvironment;

/* The following warnings are useless, because this is an abstract base class
 * and we don't know if the user maybe want to use a variable or overwrite it. */
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace LIFE.Components.Agents.BasicAgents.Agents {

  /// <summary>
  ///   An agent with a geospatial position (lat, lng).
  ///   It is placed in a GeoGrid-Environment and also has a GPS movement module.
  /// </summary>
  public abstract class GeoAgent : Agent {

    private readonly IGeoGridEnvironment<IGeoCoordinate> _env; // The grid environment to use.
    private readonly GeoPosition _position;                    // AgentReference position backing structure.
    protected readonly GeospatialMover Mover;                  // AgentReference movement module.
    public double Latitude => _position.Latitude;              // Latitude of this agent.
    public double Longitude => _position.Longitude;            // Longitude of agent position.
    public double Bearing => _position.Bearing;                // The agent's heading.


    /// <summary>
    ///   Create a new geospatial agent.
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">AgentReference registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="env">The grid environment.</param>
    /// <param name="id">The agent identifier (serialized GUID).</param>
    /// <param name="freq">MARS LIFE execution freqency.</param>
    protected GeoAgent(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt,
                              IGeoGridEnvironment<IGeoCoordinate> env, byte[] id=null, int freq=1)
      : base(layer, regFkt, unregFkt, id, freq) {
      _env = env;
      _position = new GeoPosition(0, 0);
      Mover = new GeospatialMover(env, _position, SensorArray);
    }


    /// <summary>
    ///   This function unbinds the agent from the environment.
    ///   It is triggered by the base agent, when alive flag is 'false'.
    /// </summary>
    protected override void Remove() {
      base.Remove();
      _env.Remove(_position);
    }


    /// <summary>
    ///   Return the result data for this agent.
    /// </summary>
    /// <returns>The agent's output values formatted into the result object.</returns>
    public AgentSimResult GetResultData() {
      return new AgentSimResult {
        AgentId = ID.ToString(),
        AgentType = GetType().Name,
        Layer = Layer.GetType().Name,
        Tick = GetTick(),
        Position = new [] {Longitude, Latitude},
        Orientation = new [] {Bearing, 0},
        AgentData = AgentData
      };
    }
  }
}