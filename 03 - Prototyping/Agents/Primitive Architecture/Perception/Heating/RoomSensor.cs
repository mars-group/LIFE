using Primitive_Architecture.Agents.Heating;

namespace Primitive_Architecture.Perception.Heating {
  
  /// <summary>
  ///   This sensor senses the environmental state (temperature, window).
  /// </summary>
  internal class RoomSensor : Sensor {
    private readonly TempEnvironment _environment; // The environment.

    /// <summary>
    ///   Create a new environment sensor.
    /// </summary>
    /// <param name="environment">The environment.</param>
    public RoomSensor(TempEnvironment environment)
      : base((int) Heating.InformationType.RoomState) {
      _environment = environment;
    }


    /// <summary>
    ///   Percept temperature and window open/closed changes.
    /// </summary>
    /// <returns>An input object containing the desired information.</returns>
    protected override SensorInput RetrieveData() {
      return new RoomInput(this, _environment.Temperature, _environment.WindowOpen);
    }
  }
}