using LifeAPI.Layer.Data;

namespace LifeAPI.Perception.Sensors {
  
  /// <summary>
  ///   Sensor for the grid-based object data layer.
  /// </summary>
  /// <typeparam name="T">Layer data type.</typeparam>
  public class ObjectRasterSensor<T> {

    private readonly IObjectRasterLayer<T> _layer;  // Layer reference.
    

    /// <summary>
    ///   Create a sensor for object raster layers.
    /// </summary>
    /// <param name="layer">The data layer.</param>
    public ObjectRasterSensor(IObjectRasterLayer<T> layer) {
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
