using Primitive_Architecture.Dummies;
using Primitive_Architecture.Interactions;
using Primitive_Architecture.Interfaces;

namespace Primitive_Architecture.Agents.Wolves {
  class Wolf : Agent, IAgentLogic {
    public Wolf(string id) : base(id) {
      Position = new Vector(-1, -1);
    }

    public Interaction Reason() {
      return null;
    }
  }
}
