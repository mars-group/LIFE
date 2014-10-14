using GenericAgentArchitecture.Movement;
using CommonTypes.TransportTypes;

namespace GenericAgentArchitecture.Perception {
    using LayerAPI.Interfaces;

    /// <summary>
  ///   A dummy halo. May be used as a stub for sensors with no perception limitation.
  /// </summary>
  internal class OmniHalo : Halo {
    

    /// <summary>
    ///   Create a halo that is capable of sensing everything.
    /// </summary>
    public OmniHalo() : base(new Vector(TVector.Origin.X, TVector.Origin.Y, TVector.Origin.Z)) { }

    public override AABB GetAABB() {
      return AABB.Omni;
    }

    public override bool IsInRange(TVector position) {
      return true;
    }
  }
}