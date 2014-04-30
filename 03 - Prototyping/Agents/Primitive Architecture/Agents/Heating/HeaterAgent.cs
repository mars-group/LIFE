using System;
using Primitive_Architecture.Dummies.Heating;
using Primitive_Architecture.Interfaces;

namespace Primitive_Architecture.Agents.Heating {
  internal class HeaterAgent : ITickClient {
    private double _thermalOutput;  // Actual thermal output.
    private double _thermalNominal; // Scheduled output value.
    private double _thermalChange;  // Output changing rate.

    public const int CtrMax = 5;                     // Maximum adjustment value.
    private const double ThermalMax = 2000;          // Maximum thermal output.
    private const double RiseDelay = 2;              // T1 value for PT-1 equation.
    private const double CooldownCoefficient = 0.55; // Factor to reduce cooldown speed.
    private readonly TempEnvironment _room;          // The room to heat.

    public double CtrValue { get; set; } // External applied control value.


    public HeaterAgent(TempEnvironment room) {
      _room = room;
      CtrValue = 0;
      _thermalOutput = 0;
    }


    public void Tick() {
      // Set the current output and new nominal output (based on the new setting).
      _thermalOutput = _thermalOutput + _thermalChange;
      _thermalNominal = (ThermalMax/CtrMax)*CtrValue;

      // Calculate the change of the current thermal output (delayed PT-1 element).
      _thermalChange = (_thermalNominal - _thermalOutput)/RiseDelay;
      if (_thermalChange < 0) _thermalChange *= CooldownCoefficient;

      // Set the output as thermal gain of the environment.
      _room.TempGain = _thermalOutput;
      Console.WriteLine(ToString());
    }


    public override string ToString() {
      var control = (double) ((int) (CtrValue*10))/10 + " /" + CtrMax;
      var percent = (int) Math.Round(_thermalOutput/ThermalMax*100);
      return "HeaterAgent: - Stellwert: " + control + " - Leistung: " + percent + " %";
    }
  }
}