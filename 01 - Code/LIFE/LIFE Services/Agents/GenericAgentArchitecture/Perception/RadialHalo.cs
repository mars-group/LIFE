using GenericAgentArchitecture.Dummies;

namespace GenericAgentArchitecture.Perception {
  
  /// <summary>
  ///   A halo capable of sensing in a circle around its position.
  /// </summary>
  public class RadialHalo : Halo {

    private readonly double _radius;  // The radius describing the range of this halo.


    /// <summary>
    ///   Create a circular halo.
    /// </summary>
    /// <param name="position">The agent's centre.</param>
    /// <param name="radius">The radius describing the range of this halo.</param>
    public RadialHalo(Position position, double radius) : base(position) {
      _radius = radius;
    }


    /// <summary>
    ///   Check, if a given position is inside this perception range.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True, if position is in range, false otherwise.</returns>
    public override bool IsInRange(Float3 position) {
      return Position.Center.GetDistance(position) <= _radius;
    }
  }
}