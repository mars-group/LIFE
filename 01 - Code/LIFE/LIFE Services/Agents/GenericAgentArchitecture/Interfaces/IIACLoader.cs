using System;
using System.Collections.Generic;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Interactions;

namespace GenericAgentArchitecture.Interfaces {
  public interface IIacLoader {
    List<Interaction> GetInteractions(Agent agent);
    List<Interaction> GetReflexiveActions(Agent agent);
    List<Type> GetTargetInteractions(Agent agent);
  }
}
