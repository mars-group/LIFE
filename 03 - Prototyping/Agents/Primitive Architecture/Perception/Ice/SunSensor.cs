using Primitive_Architecture.Agents.Ice;

namespace Primitive_Architecture.Perception.Ice {
  
  /// <summary>
  ///   A simple sensor to get the sunshine.
  /// </summary>
  internal class SunSensor : Sensor {

    private readonly IceWorld _environment;  // The environment to sense.


    /// <summary>
    ///   Create a new sunshine sensor.
    /// </summary>
    /// <param name="environment">The environment to sense.</param>
    public SunSensor(IceWorld environment) : base(0) {
      _environment = environment;
    }


    /// <summary>
    ///   Sense the environment for sunshine.
    /// </summary>
    /// <returns>The sensor input object.</returns>
    protected override SensorInput RetrieveData() {
      var sunshine = _environment.GetSunshine();
      return new SunInput(this, sunshine);
    }
  }
}