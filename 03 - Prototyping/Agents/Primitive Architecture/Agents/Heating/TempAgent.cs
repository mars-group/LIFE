using System;
using System.Collections.Generic;
using Primitive_Architecture.Dummies.Heating;
using Primitive_Architecture.Interaction;
using Primitive_Architecture.Perception;

namespace Primitive_Architecture.Agents.Heating {
  internal class TempAgent : Agent {
    private readonly TempEnvironment _room; // The room to heat.
    private readonly HeaterAgent _heaterAgent; // Heater used to set temperature.

    private double _lastTemp;       // The last measured temperature. Not used!
    private double _adjustment;     // Only used for status output.
    private const double TransientValue = 0.5; // Coefficient for adjustment strength.

    public double NominalTemp { get; set; }


    public TempAgent(TempEnvironment room, HeaterAgent heaterAgent, List<Sensor> sensors)
      : base("Contrl", sensors) {
      _room = room;
      _heaterAgent = heaterAgent;
      NominalTemp = 25;
    }


    /// <summary>
    /// This planning routine just evalutes the temperature offset and calculates a
    /// new heater setting. It's very simple and purely reactive ...
    /// </summary>
    /// <returns>A primitive plan with just one action: The new heater setting.</returns>
    protected override Plan CreatePlan() {
      
      // Get current temperature and calculate the deltas.
      var temp = _room.Temperature;
      var diffTemp = temp - _lastTemp;
      var diffNominal = NominalTemp - temp;

      // Here comes the black magic ...
      var newCtrl = _heaterAgent.CtrValue/HeaterAgent.CtrMax; // Current setting.
      newCtrl += (TransientValue*diffNominal/NominalTemp);    // Adjustment value.  
      if (newCtrl > 1) newCtrl = 1;                           //| Correction to fit
      if (newCtrl < 0) newCtrl = 0;                           //| percentage scale.
      if (_room.WindowOpen) newCtrl = 0;                      // Save the planet!
      _adjustment = newCtrl - (_heaterAgent.CtrValue/HeaterAgent.CtrMax);

      // Set heater control value and save temperature as reference for next run.
      _heaterAgent.CtrValue = newCtrl*HeaterAgent.CtrMax;
      _lastTemp = temp;
      
      var plan = new Plan();

      // Print output (if requested) and return the plan!
      if (DebugEnabled) Console.WriteLine(ToString());
      return plan;
    }


    /// <summary>
    /// Create a simple debug string containing the internal values of this agent.
    /// </summary>
    /// <returns>The formatted console output string.</returns>
    protected override string ToString() {
      return "Agent: " + Id + " - Sollwert: " + String.Format("{0,4:00.0}", NominalTemp) + 
             " °C - Änderung:" + String.Format("{0,5:0.0}", _adjustment*100) + " %.";
    }
  }
}
