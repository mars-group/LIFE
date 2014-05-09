using System;
using Primitive_Architecture.Agents.Heating;

namespace Primitive_Architecture.Interactions.Heating {
  class ThermalOutputInteraction : Interaction {

    private readonly TempEnvironment _environment;
    private readonly double _thermalOutput;


    public ThermalOutputInteraction(string id, TempEnvironment env, double thermalOutput)
      : base(null) {
      _environment = env;
      _thermalOutput = thermalOutput;
    }


    public override bool CheckPreconditions() {
      throw new NotImplementedException();
    }

    public override bool CheckTrigger() {
      throw new NotImplementedException();
    }

    public override void Execute() {
      _environment.TempGain = _thermalOutput;
    }
  }
}
