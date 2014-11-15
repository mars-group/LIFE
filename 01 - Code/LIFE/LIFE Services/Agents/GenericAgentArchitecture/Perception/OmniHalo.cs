using CommonTypes.TransportTypes;

namespace DalskiAgent.Perception {
    
  /// <summary>
  ///   A dummy halo. May be used as a stub for sensors with no perception limitation.
  /// </summary>
  public class OmniHalo : Halo {
    

    /// <summary>
    ///   Create a halo that is capable of sensing everything.
    /// </summary>
    public OmniHalo() : base(null) { }


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