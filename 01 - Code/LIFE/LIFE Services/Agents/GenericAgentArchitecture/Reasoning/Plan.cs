using System.Collections.Generic;
using GenericAgentArchitectureCommon.Interfaces;

namespace DalskiAgent.Reasoning {
  internal class Plan {

    private List<IInteraction> _actions; // A sequential order of the actions to execute.

    public Plan() {}


    /// <summary>
    /// Returns the next action to execute.
    /// </summary>
    /// <returns>The next action (top element in plan).</returns>
    public IInteraction GetNextAction() {
      return _actions[0];
    }
  }
}