namespace Primitive_Architecture.Perception.Heating {
  
  /// <summary>
  /// This sensor input serves as a container for all heater correlated data.
  /// </summary>
  internal class HeaterInput : SensorInput {

    public double CurValue { get; private set; }  // The current heater setting.
    public double MaxValue { get; private set; }  // The maximum setting value.

    /// <summary>
    /// Create a new heater input, containing current and maximum setting value.
    /// </summary>
    /// <param name="sensor">Sensor omitting this input.</param>
    /// <param name="curValue">The current heater setting.</param>
    /// <param name="maxValue">The maximum setting value.</param>
    public HeaterInput(Sensor sensor, double curValue, double maxValue) : base (sensor) {
      CurValue = curValue;
      MaxValue = maxValue;
    }
  }
}