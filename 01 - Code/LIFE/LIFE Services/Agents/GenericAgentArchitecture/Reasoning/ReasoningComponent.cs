using System.Collections.ObjectModel;
using GenericAgentArchitecture.Perception;
using GenericAgentArchitectureCommon.Interfaces;
using GenericAgentArchitecture.Interfaces;

namespace GenericAgentArchitecture.Reasoning {
  abstract class ReasoningComponent : IAgentLogic {

    protected ReadOnlyDictionary<int, SensorInput> Perception { get; private set; } // Perception.

    /// <summary>
    /// Constructor for the abstract reasoning component. 
    /// </summary>
    /// <param name="perception">Read-only access to the perception memory.</param>
    protected ReasoningComponent(ReadOnlyDictionary<int, SensorInput> perception) {
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
