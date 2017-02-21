using System;
using LIFE.API.GeoCommon;

namespace LIFE.Components.Agents.AgentTwo.Environment {

  /// <summary>
  ///   Position and orientation structure for geospatial entities.
  /// </summary>
  public class GeoPosition : IGeoCoordinate, IEquatable<GeoPosition> {

    private double _bearing;              // Bearing property backing field.
    public double Latitude { get; set; }  // Latitude of this agents position.
    public double Longitude { get; set; } // Longitude of this agents position.

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
    ///   Checks if this agent is at a given position.
    /// </summary>
    /// <param name="other">The position to check.</param>
    /// <returns>'True', if this agent's position equals the parameter.</returns>
    public bool Equals(IGeoCoordinate other) {
      return Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
    }


    /// <summary>
    ///   Checks if this agent equals another one.
    /// </summary>
    /// <param name="other">The other agent reference</param>
    /// <returns>'True', if this agent ID equals the other agents ID.</returns>
    public bool Equals(GeoPosition other) {
      return Equals((IGeoCoordinate)other) && _bearing.Equals(other._bearing);
    }
  }
}