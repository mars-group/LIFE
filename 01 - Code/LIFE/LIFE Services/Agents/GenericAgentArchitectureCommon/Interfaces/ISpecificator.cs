namespace GenericAgentArchitectureCommon.Interfaces {
  
  /// <summary>
  ///   Information object describing which data to query.
  /// </summary>
  public interface ISpecificator {

    /// <summary>
    ///   Return the information type specified by this object.
    /// </summary>
    /// <returns>Information type (as enum value).</returns>
    int GetInformationType();
  }
}
