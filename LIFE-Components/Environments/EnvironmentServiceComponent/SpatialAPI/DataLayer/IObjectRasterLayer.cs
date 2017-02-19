using LIFE.Components.ESC.SpatialAPI.Sensors;

namespace LIFE.Components.ESC.SpatialAPI.DataLayer {

  /// <summary>
  ///   Data layer type for arbitrary objects in a grid-based environment.
  /// </summary>
  /// <typeparam name="T">Layer data type.</typeparam>
  public interface IObjectRasterLayer<T> {


    /// <summary>
    ///   Creates a sensor for this layer and data type combination. 
    /// </summary>
    /// <returns>A sensor for this layer.</returns>
    ObjectRasterSensor<T> GetSensor(); 


    /// <summary>
    ///   Returns the raster size (relative to the global simulation coordinates). 
    /// </summary>
    /// <returns>Raster factor.</returns>
    float GetRasterFactor();


    /// <summary>
    ///   Allows direct read access to the grid.
    /// </summary>
    /// <param name="x">Grid x axis.</param>
    /// <param name="y">Grid y axis.</param>
    /// <param name="z">Height axis. Can be omitted for 2D grids.</param>
    /// <returns>Grid value.</returns>
    T this[int x, int y, int z = 0] { get; }


    /// <summary>
    ///   Returns a three-dimensional cutout of the layer data.
    ///   Notice: Cell values may be null. The size of any dimension may be 0.
    /// </summary>
    /// <param name="range">Cutout specifier.</param>
    /// <returns>Matching data sector.</returns>
    T[,,] Query(Range range);
  }
}
