using LifeAPI.Perception.Sensors;

namespace LifeAPI.Layer.Data {
  
  /// <summary>
  ///   Data layer type for single-value use cases.
  /// </summary>
  /// <typeparam name="T">Layer data type.</typeparam>
  public interface ISingleValueLayer<T> {


    /// <summary>
    ///   Creates a sensor for this layer and data type combination. 
    /// </summary>
    /// <returns>A sensor for this layer.</returns>
    SingleValueSensor<T> GetSensor();
      
      
    /// <summary>
    ///   Returns the current value of the layer.
    /// </summary>
    /// <returns>Layer value.</returns>
    T GetValue();
  }
}
