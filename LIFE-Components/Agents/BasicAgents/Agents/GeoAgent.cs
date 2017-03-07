using System;
using System.Collections.Generic;
using LIFE.API.GeoCommon;
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
  public abstract class GeoAgent<T> : Agent, IGeoAgent<T>, IEquatable<GeoAgent<T>> where T : IGeoAgent<T> {

    private readonly IGeoGridEnvironment<IGeoCoordinate> _env; // The grid environment to use.
    private GeoPosition _position;                    // AgentReference position backing structure.
    protected readonly GeoMover<T> Mover;                  // AgentReference movement module.
    public double Latitude => _position.Latitude;              // Latitude of this agent.
    public double Longitude => _position.Longitude;            // Longitude of agent position.
    public double Bearing => _position.Bearing;                // The agent's heading.

    // return 'this' in actual agent class
    public abstract T AgentReference { get; }

    internal void SetPosition(GeoPosition newPosition, double bearing = 361)
    {
      var oldBearing = _position.Bearing;
      _position = newPosition;
        if (Math.Abs(bearing - 361.0) < 0.001)
        {
            _position.Bearing = oldBearing;
        }
      _position.Bearing = bearing;
    }

    /// <summary>
    ///   Create a new geospatial agent.
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">AgentReference registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="env">The grid environment.</param>
    /// <param name="startPos">Optional starting position. If omitted, agent is not inserted.</param>
    /// <param name="id">The agent identifier (serialized GUID).</param>
    /// <param name="freq">MARS LIFE execution freqency.</param>
    protected GeoAgent(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt,
                       IGeoGridEnvironment<IGeoCoordinate> env, IGeoCoordinate startPos=null,
                       byte[] id=null, int freq=1)
      : base(layer, regFkt, unregFkt, id, freq) {
      _env = env;
      _position = new GeoPosition(0, 0);
      Mover = new GeoMover<T>(env, this, SensorArray);
        if (startPos != null)
        {
            Mover.InsertIntoEnvironment(startPos.Latitude, startPos.Longitude);
        }
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


    /// <summary>
    ///   Position comparison for the IGeoCoordinate.
    /// </summary>
    /// <param name="other">The other X/Y coordinate pair.</param>
    /// <returns>'True', if both geo coordinates sufficiently close enough.</returns>
    public bool Equals(IGeoCoordinate other) {
      const double threshold = 0.00000000000001;
      return (Math.Abs(Latitude - other.Latitude) < threshold) &&
             (Math.Abs(Longitude - other.Longitude) < threshold);
    }

      public bool Equals(GeoAgent<T> other)
      {
          if (ReferenceEquals(null, other)) return false;
          if (ReferenceEquals(this, other)) return true;
          return Equals(_position, other._position) && EqualityComparer<T>.Default.Equals(AgentReference, other.AgentReference);
      }

      public override bool Equals(object obj)
      {
          if (ReferenceEquals(null, obj)) return false;
          if (ReferenceEquals(this, obj)) return true;
          if (obj.GetType() != this.GetType()) return false;
          return Equals((GeoAgent<T>) obj);
      }

      public override int GetHashCode()
      {
          unchecked
          {
              return ((_position != null ? _position.GetHashCode() : 0) * 397) ^ EqualityComparer<T>.Default.GetHashCode(AgentReference);
          }
      }
  }
}