using GenericAgentArchitecture.Movement;

namespace GenericAgentArchitecture.Perception {
  
  /// <summary>
  ///   A dummy halo. May be used as a stub for sensors with no perception limitation.
  /// </summary>
  internal class OmniHalo : Halo {
    

    /// <summary>
    ///   Create a halo that is capable of sensing everything.
    /// </summary>
    public OmniHalo() : base(null) {}


    /// <summary>
    ///   Check, if a given position is inside this perception range.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>Always true.</returns>
    public override bool IsInRange(Vector position) {
      return true;
    }



    public override CommonTypes.DataTypes.Vector GetPosition() {
      throw new System.NotImplementedException();
    }

    public override CommonTypes.DataTypes.Vector GetDimensionQuad() {
      throw new System.NotImplementedException();
    }

    public override CommonTypes.DataTypes.Vector GetDirectionOfQuad() {
      throw new System.NotImplementedException();
    }

    public override bool IsInRange(CommonTypes.DataTypes.Vector position) {
      throw new System.NotImplementedException();
    }
  }
}