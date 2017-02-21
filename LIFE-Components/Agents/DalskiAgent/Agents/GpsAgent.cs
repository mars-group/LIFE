// // /*******************************************************
// //  * Copyright (C) Christian Hüning - All Rights Reserved
// //  * Unauthorized copying of this file, via any medium is strictly prohibited
// //  * Proprietary and confidential
// //  * This file is part of the MARS LIFE project, which is part of the MARS System
// //  * More information under: http://www.mars-group.org
// //  * Written by Christian Hüning <christianhuening@gmail.com>, 01.08.2016
// //  *******************************************************/

using System;
using System.Collections.Generic;
using LIFE.API.GeoCommon;
using LIFE.API.Layer;
using LIFE.API.Results;
using LIFE.Components.Agents.DalskiAgent.Movement;
using LIFE.Components.GeoGridEnvironment;
using MongoDB.Driver.GeoJsonObjectModel;

namespace LIFE.Components.Agents.DalskiAgent.Agents {

  /// <summary>
  ///   The GPS agent is a special implementation for the geo grid environment.
  /// </summary>
  public abstract class GpsAgent : Agent, IEquatable<GpsAgent>, IGeoCoordinate {

    private readonly ILayer _layer;   // The layer this agent lives on.
    private double _bearing;          // Bearing backup field.

    /// <summary>
    ///   Dictionary for arbitrary values. It is passed to the result database.
    /// </summary>
    protected readonly Dictionary<string, object> AgentData;

    /// <summary>
    ///   Latitude of this agents position.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    ///   Longitude of this agents position.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    ///   Agent mover for geo-grid environments.
    /// </summary>
    protected GeoGridMover Mover;


    /// <summary>
    ///   Create a new GPS agent.
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="latitude">Agent start position (latitude).</param>
    /// <param name="longitude">Agent start position (longitude).</param>
    /// <param name="geoGridEnvironment">The grid environment.</param>
    /// <param name="id">The agent ID (serialized GUID).</param>
    /// <param name="executionGroup">MARS LIFE execution quantity.</param>
    public GpsAgent(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, double latitude, double longitude,
      IGeoGridEnvironment<GpsAgent> geoGridEnvironment, byte[] id = null, int executionGroup = 1) :
        base(layer, regFkt, unregFkt, id, executionGroup) {
      AgentData = new Dictionary<string, object>();
      Latitude = latitude;
      Longitude = longitude;
      _layer = layer;
      Mover = new GeoGridMover(geoGridEnvironment, this);
      geoGridEnvironment.Insert(this);
    }


    /// <summary>
    ///   The agent orientation as compass value [0 lt. 360°].
    /// </summary>
    public double Bearing {
      get { return _bearing; }
      set {
        value %= 360;
        if (value < 0) value += 360;
        _bearing = value;
      }
    }


    /// <summary>
    ///   Checks if this agent equals another one.
    /// </summary>
    /// <param name="other">The other agent reference</param>
    /// <returns>'True', if this agent ID equals the other agents ID.</returns>
    public bool Equals(GpsAgent other) {
      return ID.Equals(other.ID);
    }


    /// <summary>
    ///   Checks if this agent is at a given position.
    /// </summary>
    /// <param name="other">The position to check.</param>
    /// <returns>'True', if this agent's position equals the parameter.</returns>
    public bool Equals(IGeoCoordinate other) {
      return Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
    }


    /// <summary>
    ///   Returns the visualization object for this agent.
    /// </summary>
    /// <returns>The agent's output values formatted into the result object.</returns>
    public virtual AgentSimResult GetResultData() {
      return new AgentSimResult {
        AgentId = ID.ToString(),
        AgentType = GetType().Name,
        Layer = _layer.GetType().Name,
        Tick = _layer.GetCurrentTick(),
        Position = GeoJson.Point(new GeoJson2DCoordinates(Longitude, Latitude)),
        Orientation = new[] {Bearing},
        AgentData = AgentData
      };
    }
  }
}