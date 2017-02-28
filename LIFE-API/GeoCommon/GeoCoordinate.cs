using System;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace LIFE.API.GeoCommon {

  /// <summary>
  ///   Copying and pasting from stack overflow:
  /// </summary>
  public class GeoCoordinate : IGeoCoordinate {

    public double Latitude { get; set; }  // Latitude of this agents position.
    public double Longitude { get; set; } // Longitude of this agents position.


    /// <summary>
    ///   Create a new GPS coordinate.
    /// </summary>
    /// <param name="latitude">Latitude value.</param>
    /// <param name="longitude">Longitude value.</param>
    public GeoCoordinate(double latitude, double longitude) {
      Latitude = latitude;
      Longitude = longitude;
    }


    /// <summary>
    ///   Checks if this position equals another one.
    /// </summary>
    /// <param name="other">The other position to check.</param>
    /// <returns>'True', if both positions are sufficiently equal.</returns>
    public bool Equals(IGeoCoordinate other) {
      const double threshold = 0.00000000000001;
      return (Math.Abs(Latitude - other.Latitude) < threshold) &&
             (Math.Abs(Longitude - other.Longitude) < threshold);
    }


    /// <summary>
    ///   Calculates the distance between two positions in km.
    /// </summary>
    /// <returns>The distance from lat lon in km.</returns>
    /// <param name="other">Coordinate to compare to.</param>
    public double Distance(GeoCoordinate other) {
      // Radius of the earth in km
      const int r = 6371;
      var dLat = Deg2Rad(other.Latitude - Latitude);
      var dLon = Deg2Rad(Longitude - other.Longitude);
      var a = Math.Sin(dLat/2)*Math.Sin(dLat/2) +
              Math.Cos(Deg2Rad(Latitude))*Math.Cos(Deg2Rad(other.Latitude))*
              Math.Sin(dLon/2)*Math.Sin(dLon/2);

      var c = 2*Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
      var d = r*c; // Distance in km
      return d;
    }

    private static double Deg2Rad(double deg) {
      return deg*(Math.PI/180);
    }

    public override string ToString() {
      return string.Format("{0},{1}", Latitude, Longitude);
    }


    public override bool Equals(object other) {
      var a = other as IGeoCoordinate;
      return (a != null) && Equals(a);
    }

    public override int GetHashCode() {
      return Latitude.GetHashCode() ^ Longitude.GetHashCode();
    }
  }
}