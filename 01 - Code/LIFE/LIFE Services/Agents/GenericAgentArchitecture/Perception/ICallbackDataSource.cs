namespace DalskiAgent.Perception {
  
  /// <summary>
  ///   This interface shall provide a data callback mechanism for passive sensors.
  /// </summary>
  public interface ICallbackDataSource {
   
 
    /// <summary>
    ///   Enables data source callback mode. Not usable at the moment!
    /// </summary>
    /// <param name="enabled">Enables or disables callback mode.</param>
    /// <param name="inputStorage">SensorInput reference for data passing.</param>
    void SetCallbackMode(bool enabled, SensorInput inputStorage);
  }
}