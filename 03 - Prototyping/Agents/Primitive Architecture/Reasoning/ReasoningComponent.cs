using System.Collections.ObjectModel;
using Common.Interfaces;
using Primitive_Architecture.Perception;
using Primitive_Architecture.Interactions;

namespace Primitive_Architecture.Reasoning {
  abstract class ReasoningComponent : IAgentLogic {

    protected ReadOnlyDictionary<int, Input> Perception { get; private set; } // Perception.

    /// <summary>
    /// Constructor for the abstract reasoning component. 
    /// </summary>
    /// <param name="perception">Read-only access to the perception memory.</param>
    protected ReasoningComponent(ReadOnlyDictionary<int, Input> perception) {
      Perception = perception;
    }


    /// <summary>
    /// An agent's reasoning process. It is called in each execution cycle after the perception
    /// phase. This is probably the most complex function in the entire program ...
    /// </summary>
    /// <returns>The interaction to execute. May be null if the agent stays idle.</returns>
    public abstract IInteraction Reason();

  }
}
