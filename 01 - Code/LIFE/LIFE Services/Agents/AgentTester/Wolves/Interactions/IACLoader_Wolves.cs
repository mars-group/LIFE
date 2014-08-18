using System;
using System.Collections.Generic;
using AgentTester.Wolves.Agents;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Dummies;
using GenericAgentArchitecture.Interactions;
using GenericAgentArchitecture.Interfaces;

namespace AgentTester.Wolves.Interactions {
  
  internal class IACLoaderWolves : IIacLoader {
    
    public List<Interaction> GetInteractions(Agent agent) {
      if (agent is Wolf || agent is Sheep) return new List<Interaction> {
        new EatInteraction(null, null), new ProcreateInteraction(null)
      };
      return new List<Interaction>();
    }


    public List<Interaction> GetReflexiveActions(Agent agent) {
      if (agent is Wolf || agent is Sheep) return new List<Interaction> {
        new MoveInteraction(null, new Float3(0, 0))
      };
      if (agent is Grass) return new List<Interaction> {
        new GrowInteraction(null)
      };
      return new List<Interaction>();
    }


    public List<Type> GetTargetInteractions(Agent agent) {
      if (agent is Wolf) return new List<Type> {
        typeof(ProcreateInteraction)
      };
      if (agent is Sheep) return new List<Type> {
        typeof(EatInteraction), typeof(ProcreateInteraction)
      };
      if (agent is Grass) return new List<Type> {
        typeof(EatInteraction)
      };
      return new List<Type>();
    }
  }
}