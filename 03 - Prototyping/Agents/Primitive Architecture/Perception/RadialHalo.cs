using System;
using Primitive_Architecture.Dummies;

namespace Primitive_Architecture.Perception {
  
  /// <summary>
  ///   A halo capable of sensing in a circle around its position.
  /// </summary>
  internal class RadialHalo : Halo {

    private readonly double _radius;  // The radius describing the range of this halo.


    /// <summary>
    ///   Create a circular halo.
    /// </summary>
    /// <param name="position">The agent's centre.</param>
    /// <param name="radius">The radius describing the range of this halo.</param>
    public RadialHalo(Vector position, double radius) : base(position) {
      _radius = radius;
    }


    /// <summary>
    ///   Check, if a given position is inside this perception range.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True, if position is in range, false otherwise.</returns>
    public override bool IsInRange(Vector position) {
      return _radius <= Position.GetDistance(position);
    }
  }
}