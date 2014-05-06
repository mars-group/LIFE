using System.Collections.Generic;

namespace Primitive_Architecture.Interactions {
  internal class Plan {

    private List<Interaction> _actions; // A sequential order of the actions to execute.

    public Plan() {
    
    }


    /// <summary>
    /// Returns the next action to execute.
    /// </summary>
    /// <returns>The next action (top element in plan).</returns>
    public Interaction GetNextAction() {
      return _actions[0];
    }
  }
}