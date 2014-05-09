using System;
using Common.Interfaces;
using Primitive_Architecture.Interactions;
using Primitive_Architecture.Interactions.Heating;

namespace Primitive_Architecture.Agents.Heating {

  /// <summary>
  /// This agent represents a heater. It is a primitive reactive agent with no input.
  /// </summary>
  internal class HeaterAgent : Agent, IAgentLogic {

    private double _thermalOutput;  // Actual thermal output.
    private double _thermalNominal; // Scheduled output value.
    private double _thermalChange;  // Output changing rate.

    public const int CtrMax = 5;                     // Maximum adjustment value.
    public double CtrValue { get; set; }             // External applied control value.       
    private const double ThermalMax = 2000;          // Maximum thermal output.
    private const double RiseDelay = 2;              // T1 value for PT-1 equation.
    private const double CooldownCoefficient = 0.55; // Factor to reduce cooldown speed.
    private readonly TempEnvironment _room;          // The room to heat.

    /// <summary>
    /// Create a heater agent. This is basically an environmental object.
    /// Interactions: (1,0) [S] Produce thermal output.
    ///               (1,1) [T] Change heater setting.
    /// </summary>
    /// <param name="room">The room to heat.</param>
    public HeaterAgent(TempEnvironment room) : base ("Heater") {
      _room = room;
      CtrValue = 0;
      _thermalOutput = 0;
      ReasoningComponent = this; // This simple agent has no external reasoning component!
    }


    /// <summary>
    /// A simple calculation of new output value and creation of interaction object.
    /// </summary>
    /// <returns>The heating interaction.</returns>
    public IInteraction Reason () {
      
      // Set the current output and new nominal output (based on the new setting).
      _thermalOutput = _thermalOutput + _thermalChange;
      _thermalNominal = (ThermalMax/CtrMax)*CtrValue;

      // Calculate the change of the current thermal output (delayed PT-1 element).
      _thermalChange = (_thermalNominal - _thermalOutput)/RiseDelay;
      if (_thermalChange < 0) _thermalChange *= CooldownCoefficient;

      // Set the output as thermal gain of the environment.
      return new ThermalOutputInteraction("", _room, _thermalOutput);
    }


    /// <summary>
    /// Print nominal setting and performance. 
    /// </summary>
    /// <returns>Console output string.</returns>
    protected override string ToString() {
      var control = (double) ((int) (CtrValue*10))/10 + " /" + CtrMax;
      var percent = (int) Math.Round(_thermalOutput/ThermalMax*100);
      return "Agent: "+Id+" - Stellwert: " + control + " - Leistung: " + percent 
        + " % (Soll: "+((int)(CtrValue*100/CtrMax))+" %)";
    }
  }
}