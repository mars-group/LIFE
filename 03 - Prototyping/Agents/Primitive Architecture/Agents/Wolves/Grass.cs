using Common.Interfaces;
using Primitive_Architecture.Dummies;
using Primitive_Architecture.Interactions;
using Primitive_Architecture.Interfaces;

namespace Primitive_Architecture.Agents.Wolves {
  class Grass : Agent, IAgentLogic {
    public Grass(string id) : base(id) {
      Position = new Vector(-1, -1);
    }

    public IInteraction Reason() {
      return null;
    }
  }
}
