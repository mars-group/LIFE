using System;
using Primitive_Architecture.Interactions;
using Primitive_Architecture.Interfaces;
using Primitive_Architecture.Perception.Ice;

namespace Primitive_Architecture.Agents.Ice {
  
  internal class Iceman : Agent, IAgentLogic {

    private bool _sun;

    public Iceman(IceWorld environment) : base("Iceman") {
      PerceptionUnit.AddSensor(new SunSensor(environment));
      // ReasoningComponent = new Goap();
    }

    //TODO Dummy only!
    public Interaction Reason() {
      var sunInput = (SunInput) PerceptionUnit.GetData(0);
      _sun = sunInput.GetSunshine();
      return null;
    }


    protected override string ToString () {
      return "[" + Cycle + "] [Iceman]   Sun shines"+(_sun? "!" : " not. :-(");
    }
  }
}