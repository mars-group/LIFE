using LifeAPI.Layer.Data;

namespace LifeAPI.Perception.Sensors {
  
  /// <summary>
  ///   Sensor for single value data layers.
  /// </summary>
  /// <typeparam name="T">Layer data type.</typeparam>
  public class SingleValueSensor<T> {

    private ISingleValueLayer<T> _layer;  // Layer reference.


    /// <summary>
    ///   Create a sensor for single value grids.
    /// </summary>
    /// <param name="layer">The data layer.</param>
    public SingleValueSensor(ISingleValueLayer<T> layer) {
      _layer = layer;
    } 


    /// <summary>
    ///   Returns the current value of the layer.
    /// </summary>
    /// <returns>Layer value.</returns>
    public T GetValue() {
      return _layer.GetValue();
    } 
  }
}
