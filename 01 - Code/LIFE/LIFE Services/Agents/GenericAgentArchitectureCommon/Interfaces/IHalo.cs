using CommonTypes.TransportTypes;

namespace GenericAgentArchitectureCommon.Interfaces {
  
  /// <summary>
  ///   This interface demands the range check for object perception.   
  /// </summary>
  public interface IHalo {


    /// <summary>
    ///   Check, if a given position is inside this perception range.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True, if position is in range, false otherwise.</returns>
    bool IsInRange(TVector position);
  }
}
