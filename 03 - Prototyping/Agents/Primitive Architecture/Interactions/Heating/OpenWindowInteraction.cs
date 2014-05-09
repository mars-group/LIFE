using System;
using Primitive_Architecture.Agents.Heating;

namespace Primitive_Architecture.Interactions.Heating {
  class OpenWindowInteraction : Interaction {

    private readonly TempEnvironment _environment;
    private readonly bool _open;


    public OpenWindowInteraction(string id, TempEnvironment env, bool open)
      : base(null) {
      _environment = env;
      _open = open;
    }


    public override bool CheckPreconditions() {
      throw new NotImplementedException();
    }

    public override bool CheckTrigger() {
      throw new NotImplementedException();
    }

    public override void Execute() {
      _environment.WindowOpen = _open;
    }
  }
}
