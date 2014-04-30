namespace Primitive_Architecture.Perception {
  
  /// <summary>
  /// This class specifies the generic input for a sensory input type.
  /// </summary>
  internal abstract class SensorInput : Input {

    public Sensor OriginSensor { get; private set; }

    public SensorInput(Sensor originSensor) {
      OriginSensor = originSensor;
    }

  }
}