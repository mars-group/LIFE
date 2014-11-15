﻿using CommonTypes.TransportTypes;
using DalskiAgent.Environments;

namespace DalskiAgent.Perception {

  /// <summary>
  ///   A halo capable of sensing in a circle around its position.
  /// </summary>
  public class RadialHalo : Halo {

    private readonly float _radius; // The radius describing the range of this halo.


    /// <summary>
    ///   Create a circular halo.
    /// </summary>
    /// <param name="data">The agent's R/O data container.</param>
    /// <param name="radius">The radius describing the range of this halo.</param>
    public RadialHalo(DataAccessor data, float radius) : base(data) {
      _radius = radius;
    }


    /// <summary>
    ///   Check, if a given position is inside this perception range.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True, if position is in range, false otherwise.</returns>
    public override bool IsInRange(TVector position) {
      var dist = Data.Position.GetTVector().GetDistance(position);
      return (dist <= _radius) && (dist > float.Epsilon);
    }
  }
}