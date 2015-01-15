using LifeAPI.Perception.Sensors;

namespace LifeAPI.Layer.Data {

  /// <summary>
  ///   Data layer type for primitive values only (int, float, ...) that are arranged in a grid.
  /// </summary>
  /// <typeparam name="T">Layer data type (constrained to value types).</typeparam>
  public interface INumericRasterLayer<T> where T : struct {


    /// <summary>
    ///   Creates a sensor for this layer and data type combination. 
    /// </summary>
    /// <returns>A sensor for this layer.</returns>
    NumericRasterSensor<T> GetSensor(); 


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
