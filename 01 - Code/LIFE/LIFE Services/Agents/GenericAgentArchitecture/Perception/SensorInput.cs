namespace GenericAgentArchitecture.Perception {
  
  /// <summary>
  /// This class specifies the generic input for a sensory input type.
  /// </summary>
  internal abstract class SensorInput : Input {

    public Sensor OriginSensor { get; private set; } // The creator of this SensorInput.

    /// <summary>
    /// Create a new sensor input (called by more specific input types).
    /// </summary>
    /// <param name="originSensor">The creator of this sensor input.</param>
    protected SensorInput(Sensor originSensor) {
      OriginSensor = originSensor;
    }

  }
}