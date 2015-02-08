using SpatialAPI.Entities;

namespace DalskiAgent.Agents {

  /// <summary>
  ///   Spatial agent public methods.
  /// </summary>
  public interface ISpatialAgent {

    /// <summary>
    ///   Return the spatial entity of this agent.
    /// </summary>
    ISpatialEntity Entity { get; }    
  }
}