using Primitive_Architecture.Agents.Heating;

namespace Primitive_Architecture.Interactions.Heating {
  class AdjustSettingInteraction : Interaction {

    private readonly HeaterAgent _heater;
    private readonly double _ctrValue;


    public AdjustSettingInteraction(string id, HeaterAgent heater, double ctrValue)
      : base(id, null, null) {
      _heater = heater;
      _ctrValue = ctrValue;
    }
    
    
    public override bool IsExecutable() {
      return true;
    }

    
    public override void Execute() {
      _heater.CtrValue = _ctrValue;
    }
  }
}
