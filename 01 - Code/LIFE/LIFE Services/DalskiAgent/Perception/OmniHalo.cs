﻿using System;
using LifeAPI.Spatial;

namespace DalskiAgent.Perception {

  /// <summary>
  ///   A dummy halo. May be used as a stub for sensors with no perception limitation.
  /// </summary>
  public class OmniHalo : Halo {
    

    /// <summary>
    ///   Create a halo that is capable of sensing everything.
    /// </summary>
    /// <param name="informationType">The information type to perceive.</param>
    public OmniHalo(Enum informationType) : base(null, informationType) { }


        /// <summary>
        ///   Check, if a given position is inside this perception range.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>True, if position is in range, false otherwise.</returns>
        public override bool IsInRange(TVector position) {
      return true;
    }
  }
}