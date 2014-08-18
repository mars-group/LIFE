using System;
using GenericAgentArchitecture.Interactions;
using GenericAgentArchitecture.Interfaces;

namespace AgentTester.Wolves.Interactions {
  
  class ProcreateInteraction : Interaction {
    
    public ProcreateInteraction(IGenericAPI source) : base(source) {}
    
    public override bool CheckPreconditions() {
      throw new NotImplementedException();
    }

    public override bool CheckTrigger() {
      throw new NotImplementedException();
    }

    public override void Execute() {
      throw new NotImplementedException();
    }
  }
}
