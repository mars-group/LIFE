using Primitive_Architecture.Agents.Heating;

namespace Primitive_Architecture.Interactions.Heating {
  class ThermalOutputInteraction : Interaction {

    private readonly TempEnvironment _environment;
    private readonly double _thermalOutput;


    public ThermalOutputInteraction(string id, TempEnvironment env, double thermalOutput)
      : base(id, null, null) {
      _environment = env;
      _thermalOutput = thermalOutput;
    }


    public override bool IsExecutable() {
      return true;     
    }


    public override void Execute() {
      _environment.TempGain = _thermalOutput;
    }
  }
}
