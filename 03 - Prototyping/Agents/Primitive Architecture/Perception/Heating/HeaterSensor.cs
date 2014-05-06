using Primitive_Architecture.Agents.Heating;

namespace Primitive_Architecture.Perception.Heating {
  
  /// <summary>
  ///   The heater sensor polls the heater settings and returns them to the PU.
  /// </summary>
  internal class HeaterSensor : Sensor {
    private readonly HeaterAgent _heater; // The heater to be polled.

    /// <summary>
    ///   Create a new heater sensor.
    /// </summary>
    /// <param name="heater">The heater to be polled.</param>
    public HeaterSensor(HeaterAgent heater) : base((int) Heating.InformationType.HeaterSetting) {
      _heater = heater;
    }


    /// <summary>
    ///   Gather current and maximum heater setting.
    /// </summary>
    /// <returns>An input object containing the desired information.</returns>
    protected override SensorInput RetrieveData() {
      return new HeaterInput(this, _heater.CtrValue, HeaterAgent.CtrMax);
    }
  }
}