using SpatialAPI.DataLayer;

namespace SpatialAPI.Sensors {
  
  /// <summary>
  ///   Sensor for numeric raster layers.
  /// </summary>  
  /// <typeparam name="T">Layer data type.</typeparam>
  public class NumericRasterSensor<T> where T : struct {

    private INumericRasterLayer<T> _layer;  // Layer reference.


    /// <summary>
    ///   Create a sensor for numeric grids.
    /// </summary>
    /// <param name="layer">The data layer.</param>
    public NumericRasterSensor(INumericRasterLayer<T> layer) {
      _layer = layer;
    } 
      
      
    /// <summary>
    ///   Allows direct read access to the grid.
    /// </summary>
    /// <param name="x">Grid x axis.</param>
    /// <param name="y">Grid y axis.</param>
    /// <param name="z">Height axis. Can be omitted for 2D grids.</param>
    /// <returns>Grid value.</returns>
    public T this[int x, int y, int z = 0] {
      get {
        return _layer[x, y, z];
      }
    }


    /// <summary>
    ///   Returns a three-dimensional cutout of the layer data.
    ///   Notice: Cell values may be null. The size of any dimension may be 0.
    /// </summary>
    /// <param name="range">Cutout specifier.</param>
    /// <returns>Matching data sector.</returns>
    public T[,,] Query(Range range) {
      return _layer.Query(range);
    }  
  }
}
