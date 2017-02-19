namespace LIFE.Components.DalskiAgent.Interactions {
  
  /// <summary>
  ///   Interaction interface. Just ensures executability,
  ///   needed by the abstract base agent.
  /// </summary>
  public interface IInteraction {
    
    /// <summary>
    ///   Execute the interaction object.
    /// </summary>
    void Execute();
  }
}