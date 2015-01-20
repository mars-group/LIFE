namespace DalskiAgent.Reasoning {
  
  abstract class ReasoningComponent : IAgentLogic {

    /// <summary>
    /// An agent's reasoning process. It is called in each execution cycle after the perception
    /// phase. This is probably the most complex function in the entire program ...
    /// </summary>
    /// <returns>The interaction to execute. May be null if the agent stays idle.</returns>
    public abstract IInteraction Reason();

  }
}
