using System;
using System.Collections.Generic;
using Primitive_Architecture.Agents;
using Primitive_Architecture.Interactions;

namespace Primitive_Architecture.Interfaces {
  interface IIACLoader {
    List<Interaction> GetInteractions(Agent agent);
    List<Interaction> GetReflexiveActions(Agent agent);
    List<Type> GetTargetInteractions(Agent agent);
  }
}
