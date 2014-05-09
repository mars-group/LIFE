using Common.Types;

namespace Primitive_Architecture.Perception.Ice {
  
  /// <summary>
  ///   Storage object for sunshine sensor input.
  /// </summary>
  internal class SunInput : SensorInput, ISunInput {

    private readonly bool _sunshine;   // Flag for sunshine value.


    /// <summary>
    ///   Create a new sunshine input object.
    /// </summary>
    /// <param name="originSensor">The sunshine sensor.</param>
    /// <param name="sunshine">Input value: True=Sunshine, false=cloudy.</param>
    public SunInput(Sensor originSensor, bool sunshine) : base(originSensor) {
      _sunshine = sunshine;
    }


    /// <summary>
    ///   Return the sunshine value.
    /// </summary>
    /// <returns>A boolean: True, if sun shines, else false.</returns>
    public bool GetSunshine() {
      return _sunshine;
    }
  }
}