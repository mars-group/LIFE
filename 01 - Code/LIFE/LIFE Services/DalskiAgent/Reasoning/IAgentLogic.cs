namespace DalskiAgent.Reasoning {
  
  /// <summary>
  ///   This interface specifies the agent reasoning function. 
  /// </summary>
  public interface IAgentLogic {
    
    /// <summary>
    ///   Reasoning logic method.
    /// </summary>
    /// <returns>The interaction the agent shall execute.</returns>
    IInteraction Reason();
  }
}
