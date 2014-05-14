using Common.Interfaces;
using Common.Types;
using Primitive_Architecture.Perception.Ice;

namespace Primitive_Architecture.Agents.Ice {
  
  internal class Iceman : Agent, IAgentLogic {

    private bool _sun;

    public Iceman(IceWorld environment) : base("Iceman") {
      PerceptionUnit.AddSensor(new SunSensor(environment));
      //ReasoningComponent = new Goap();
    }


    //TODO Dummy only!
    public IInteraction Reason() {
      //TODO Interface-Typ übergeben.
      var sunInput = PerceptionUnit.GetData<SunInput>();
      _sun = sunInput.GetSunshine();
      return null;
    }


    protected override string ToString () {
      return "[" + Cycle + "] [Iceman]   Sun shines"+(_sun? "!" : " not. :-(");
    }
  }
}