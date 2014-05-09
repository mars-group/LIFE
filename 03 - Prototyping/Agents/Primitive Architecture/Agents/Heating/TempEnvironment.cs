using System;
using Primitive_Architecture.Interactions.Wolves;
using Primitive_Architecture.Interfaces;
using Primitive_Architecture.Perception;
using Environment = Primitive_Architecture.Dummies.Environment;

namespace Primitive_Architecture.Agents.Heating {
  internal class TempEnvironment : Environment, IGenericDataSource, ICallbackDataSource {
    
    private const double Temp1 = 15;
    private const double Temp2 = 28;
    private const double Thermal1 = 0;
    private const double Thermal2 = 2000;
    private const double WindowInfl = 2;

    public double TempGain { set; private get; }
    public bool WindowOpen { get; set; }
    public double Temperature { get; private set; }


    public TempEnvironment() : base (new IACLoaderWolves()) {
      Temperature = 15;
      WindowOpen = false;
      TempGain = 0;
    }


    protected override void AdvanceEnvironment() {
      // Thermal value corresponding to one degree Celsius.
      const double tempUnit = (Thermal2 - Thermal1)/(Temp2 - Temp1);

      // Environmental cool off - increased, if window is opened.
      var tempLoss = (Temperature - Temp1)*tempUnit;
      if (WindowOpen) tempLoss *= 1 + ((Temperature - Temp1)/(Temp2 - Temp1))*WindowInfl;

      // Summarize thermal gain and loss to get the new temperature.
      Temperature = Temperature + 0.5*(TempGain - tempLoss)/tempUnit;

      // Parameter, Spaces total (- = left-aligned, digits before . digits after)
      Console.WriteLine("Agent: Room   - Temperatur: " + String.Format("{0,4:00.0}", Temperature)
                        + " °C (" + (int) TempGain + " / " + (int) tempLoss +
                        ") - Fenster: " + (WindowOpen ? "offen" : "geschlossen") + ".");
    }


    public override double GetDistance(Agent x, Agent y) {
      throw new NotImplementedException();
    }


    //TODO 0=Temp, 1=HeaterAgent, 2=WindowState
    public SensorInput GetData (int dataType) {
      var gsi = new GenericSensorInput(null); 
      switch (dataType) {
        case 0: gsi.Values.Add("Temperature", Temperature); break;
        //case 1: gsi.Values.Add("HeatingLevel", null); break;
        case 2: gsi.Values.Add("WindowOpen", WindowOpen); break;
      }
      return gsi;
    }


    public void SetCallbackMode(bool enabled, SensorInput inputStorage) {
      throw new NotImplementedException();
    }
  }
}