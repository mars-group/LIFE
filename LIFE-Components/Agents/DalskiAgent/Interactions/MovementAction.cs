namespace LIFE.Components.Agents.DalskiAgent.Interactions {
  
  /// <summary>
  ///   Action object for agent movement.
  /// </summary>
  public class MovementAction : IInteraction {

    private readonly MovementFunction _movementFkt;  // Movement function to execute.

    /// <summary>
    ///   Create a new movement action.
    /// </summary>
    /// <param name="movementFkt">Movement function to execute.</param>
    public MovementAction(MovementFunction movementFkt) {
      _movementFkt = movementFkt;
    }


    /// <summary>
    ///   Executes the movement.
    /// </summary>
    public void Execute() {
      _movementFkt();
    }


    /// <summary>
    ///   This function holds the instructions how to perform this movement.
    ///   It is set in the concrete mover modules.
    /// </summary>
    public delegate void MovementFunction();
  }
}