using GenericAgentArchitecture.Dummies;

namespace GenericAgentArchitecture.Perception {
  
  /// <summary>
  ///   An abstract halo representation. Each sensor has one object of it.
  /// </summary>
  public abstract class Halo {
    public Position Position; // The agent's centre.
    //private Geometry _form;


    /// <summary>
    ///   Create a new halo.
    /// </summary>
    /// <param name="position">The agent's centre.</param>
    protected Halo(Position position) {
      Position = position;
    }


    /// <summary>
    ///   Check, if a given position is inside this perception range.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True, if position is in range, false otherwise.</returns>
    public abstract bool IsInRange(Float3 position);
  }
}