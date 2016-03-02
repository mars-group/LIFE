namespace LifeAPI.Results {

  /// <summary>
  ///   All simulation entities that produce simulation results intended
  ///   to be stored in the database should implement this interface. 
  /// </summary>
  public interface ISimResult {
    
    
    /// <summary>
    ///   Returns an appropriately formatted JSON string.
    /// </summary>
    /// <returns>Agent properties in JSON notation.</returns>
    string GetResultData();
  }
}