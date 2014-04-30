namespace Primitive_Architecture.Perception.Heating {
 
  /// <summary>
  /// All temperature related information goes here.
  /// </summary>
  internal class RoomInput : Input {
    
    public double Temperature { get; private set; } // The current temperature.
    public bool  WindowOpened { get; private set; } // Is the window opened?

    /// <summary>
    /// Create a new temperature input.
    /// </summary>
    /// <param name="temperature">The current temperature.</param>
    /// <param name="opened">Is the window opened?</param>
    public RoomInput(double temperature, bool opened) {
      Temperature = temperature;
      WindowOpened = opened;
    }
  }
}