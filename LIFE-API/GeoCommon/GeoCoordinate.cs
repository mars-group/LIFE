using System;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace LIFE.API.GeoCommon {

  /// <summary>
  ///   Copying and pasting from stack overflow:
  ///   http://stackoverflow.com/questions/6151625/should-i-use-a-struct-or-a-class-to-represent-a-lat-lng-coordinate
  /// </summary>
  public class GeoCoordinate : IGeoCoordinate {

    public GeoCoordinate(double latitude, double longitude) {
      Latitude = latitude;
      Longitude = longitude;
    }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public bool Equals(IGeoCoordinate other) {
      return (Math.Abs(Latitude - other.Latitude) < 0.00000000000001) &&
             (Math.Abs(Longitude - other.Longitude) < 0.00000000000001);
    }

    /// <summary>
    ///   Calculates the distance between two positions in km.
    ///   Copying and Pasting from stack overflow for dummies:
    ///   http://stackoverflow.com/questions/27928/calculate-distance-between-two-latitude-longitude-points-haversine-formula
    /// </summary>
    /// <returns>The distance from lat lon in km.</returns>
    /// <param name="other">Coordinate to compare to.</param>
    public double Distance(GeoCoordinate other) {
      // Radius of the earth in km
      var R = 6371;
      var dLat = Deg2Rad(other.Latitude - Latitude);
      var dLon = Deg2Rad(Longitude - other.Longitude);
      var a =
          Math.Sin(dLat/2)*Math.Sin(dLat/2) +
          Math.Cos(Deg2Rad(Latitude))*Math.Cos(Deg2Rad(other.Latitude))*
          Math.Sin(dLon/2)*Math.Sin(dLon/2)
        ;
      var c = 2*Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
      var d = R*c; // Distance in km
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