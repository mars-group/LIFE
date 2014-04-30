namespace Primitive_Architecture.Perception {
  
  internal abstract class SensorInput : Input {

    public Sensor OriginSensor { get; private set; }

    public SensorInput(Sensor originSensor) {
      OriginSensor = originSensor;
    }

  }
}