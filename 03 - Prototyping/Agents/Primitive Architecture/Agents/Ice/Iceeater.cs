using System;
using Common.Interfaces;
using Primitive_Architecture.Interactions;
using Primitive_Architecture.Interfaces;
using Primitive_Architecture.Perception.Ice;

namespace Primitive_Architecture.Agents.Ice {

  internal class Iceeater : Agent, IAgentLogic {

    private bool _sun;

    public Iceeater(IceWorld environment) : base("Iceeater") {
      PerceptionUnit.AddSensor(new SunSensor(environment));
      // ReasoningComponent = new Goap();
    }

    //TODO Dummy only!
    public IInteraction Reason() {
      var sunInput = PerceptionUnit.GetData<SunInput>();
      _sun = sunInput.GetSunshine();
      return null;
    }


    protected override string ToString() {
      return "[" + Cycle + "] [Iceeater] Sun shines" + (_sun ? "!" : " not. :-(");
    }
  }
}