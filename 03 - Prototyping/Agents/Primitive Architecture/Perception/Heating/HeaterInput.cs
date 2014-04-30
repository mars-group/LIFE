namespace Primitive_Architecture.Perception.Heating {
  
  /// <summary>
  /// This sensor input serves as a container for all heater correlated data.
  /// </summary>
  internal class HeaterInput : Input {

    public double CurValue { get; private set; }  // The current heater setting.
    public double MaxValue { get; private set; }  // The maximum setting value.

    /// <summary>
    /// Create a new heater input, containing current and maximum setting value.
    /// </summary>
    /// <param name="curValue">The current heater setting.</param>
    /// <param name="maxValue">The maximum setting value.</param>
    public HeaterInput(double curValue, double maxValue) {
      CurValue = curValue;
      MaxValue = maxValue;
    }
  }
}