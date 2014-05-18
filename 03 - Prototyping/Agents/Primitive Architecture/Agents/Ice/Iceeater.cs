using Common.Interfaces;
using Common.Types;
using GoapComponent;
using Primitive_Architecture.Perception.Ice;

namespace Primitive_Architecture.Agents.Ice {

  internal class Iceeater : Agent, IAgentLogic {

    private bool _sun;

    public Iceeater(IceWorld environment) : base("Iceeater") {
      PerceptionUnit.AddSensor(new SunSensor(environment));
      ReasoningComponent = new Goap(PerceptionUnit);
    }

    //TODO Dummy only!
    public IInteraction Reason() {
      var sunInput = PerceptionUnit.GetData<ISunInput>();
      _sun = sunInput.GetSunshine();
      return null;
    }


    public override string ToString() {
      return "[" + Cycle + "] [Iceeater] Sun shines" + (_sun ? "!" : " not. :-(");
    }
  }
}