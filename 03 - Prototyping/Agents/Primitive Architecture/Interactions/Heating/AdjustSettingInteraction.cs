using System;
using Primitive_Architecture.Agents.Heating;

namespace Primitive_Architecture.Interactions.Heating {
  class AdjustSettingInteraction : Interaction {

    private readonly HeaterAgent _heater;
    private readonly double _ctrValue;


    public AdjustSettingInteraction(string id, HeaterAgent heater, double ctrValue)
      : base(null) {
      _heater = heater;
      _ctrValue = ctrValue;
    }


    public override bool CheckPreconditions() {
      throw new NotImplementedException();
    }

    public override bool CheckTrigger() {
      throw new NotImplementedException();
    }

    public override void Execute() {
      _heater.CtrValue = _ctrValue;
    }
  }
}
