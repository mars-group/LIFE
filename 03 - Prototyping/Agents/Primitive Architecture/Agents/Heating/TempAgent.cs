using System;
using Primitive_Architecture.Interactions;
using Primitive_Architecture.Interactions.Heating;
using Primitive_Architecture.Interfaces;
using Primitive_Architecture.Perception.Heating;

namespace Primitive_Architecture.Agents.Heating {

  /// <summary>
  /// A heater controlling instance. The governing process is not in the least intelligent ...
  /// </summary>
  internal class TempAgent : Agent, IAgentLogic {

    private readonly HeaterAgent _heaterAgent; // Heater used to set temperature.
    private double _adjustment;                // Only used for status output.
    private readonly double _transientValue;   // Coefficient for adjustment strength.
    private readonly double _nominalTemp;      // The desired temperature.


    /// <summary>
    /// Create a new heater controller.
    /// </summary>
    /// <param name="room">The room to heat.</param>
    /// <param name="heaterAgent">Heater used to set temperature.</param>
    public TempAgent(TempEnvironment room, HeaterAgent heaterAgent) : base("Contrl") {
      _heaterAgent = heaterAgent;
      _nominalTemp = 25;
      _transientValue = 0.5;
      ReasoningComponent = this;
      PerceptionUnit.AddSensor(new RoomSensor(room));
      PerceptionUnit.AddSensor(new HeaterSensor(heaterAgent));
    }


    /// <summary>
    /// This planning routine just evalutes the temperature offset and calculates a
    /// new heater setting. It's very simple and purely reactive ...
    /// </summary>
    /// <returns>An action to adjust the heater setting.</returns>
    public Interaction Reason() {
      
      // Get current temperature and calculate the deltas.
      var roomInput = PerceptionUnit.GetData<RoomInput>();
      var temp = roomInput.Temperature;
      var windowOpen = roomInput.WindowOpened;
      var diffNominal = _nominalTemp - temp;

      // Get the heater settings.
      var heaterInput = PerceptionUnit.GetData<HeaterInput>();
      double maxValue = heaterInput.MaxValue;
      double curValue = heaterInput.CurValue;

      // Here comes the black magic ...
      var newCtrl = curValue / maxValue;                      // Current setting.
      newCtrl += (_transientValue*diffNominal/_nominalTemp);  // Adjustment value.  
      if (newCtrl > 1) newCtrl = 1;                           //| Correction to fit
      if (newCtrl < 0) newCtrl = 0;                           //| percentage scale.
      if (windowOpen) newCtrl = 0;                            // Save the planet!
      _adjustment = newCtrl - (curValue / maxValue);

      // Set heater control value.
      return new AdjustSettingInteraction("", _heaterAgent, newCtrl*HeaterAgent.CtrMax); 
    }


    /// <summary>
    /// Create a simple debug string containing the internal values of this agent.
    /// </summary>
    /// <returns>The formatted console output string.</returns>
    protected override string ToString() {
      return "Agent: " + Id + " - Sollwert: " + String.Format("{0,4:00.0}", _nominalTemp) + 
             " °C - Änderung:" + String.Format("{0,5:0.0}", _adjustment*100) + " %.";
    }
  }
}
