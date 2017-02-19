using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;

namespace LIFE.Components.DalskiAgent.Movement {
  
  /// <summary>
  ///   Interface class for spatial queries on the agent.
  /// </summary>
  public interface IAgentEntity {


    /// <summary>
    ///   The position of the agent.
    /// </summary>
    Vector3 Position { get; }
    

    /// <summary>
    ///   Dimension of this agent's bounding box.
    /// </summary>    
    Vector3 Dimension { get; }
    
    
    /// <summary>
    ///   The agent's heading.
    /// </summary>
    Direction Direction { get; }


    /// <summary>
    ///   Public readable agent information string for various purposes. 
    /// </summary>
    string AgentInformation { get; }
  }
}