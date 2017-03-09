namespace LIFE.API.Environment.GeoCommon {

  /// <summary>
  ///   Position and orientation structure for geospatial entities.
  /// </summary>
  public class GeoPosition : GeoCoordinate {

    private double _bearing;  // Bearing property backing field.


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
    ///   Create a new GeoPosition.
    /// </summary>
    /// <param name="lat">Latitude value.</param>
    /// <param name="lng">Longitude value.</param>
    public GeoPosition(double lat, double lng) : base(lat, lng) {}
  }
}