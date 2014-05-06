namespace Primitive_Architecture.Perception.Heating {
 
  /// <summary>
  /// All temperature related information goes here.
  /// </summary>
  internal class RoomInput : SensorInput {
    
    public double Temperature { get; private set; } // The current temperature.
    public bool  WindowOpened { get; private set; } // Is the window opened?

    /// <summary>
    /// Create a new temperature input.
    /// </summary>
    /// <param name="sensor">Sensor omitting this input.</param>
    /// <param name="temperature">The current temperature.</param>
    /// <param name="opened">Is the window opened?</param>
    public RoomInput(Sensor sensor, double temperature, bool opened) : base (sensor) {
      Temperature = temperature;
      WindowOpened = opened;
    }
  }
}