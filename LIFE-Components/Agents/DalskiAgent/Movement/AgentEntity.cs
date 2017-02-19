using System;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.DalskiAgent.Movement {
  
  /// <summary>
  ///   Agent-internal ISpatialEntity implementation.
  /// </summary>
  [Serializable]
  public class AgentEntity : IAgentEntity, ISpatialEntity {


    //_________________________________________________________________________
    // ISpatialEntity related methods for usage with IEnvironment instances.     

    /// <summary>
    ///   A geometric shape describing this agent's body.
    /// </summary>
    public IShape Shape { get; set; }


    /// <summary>
    ///   This agent's collision type.
    /// </summary>
    public Enum CollisionType { get; set; }


    /// <summary>
    ///   The ID of the base agent.
    /// </summary>
    public Guid AgentGuid { get; set; }


    /// <summary>
    ///   Type of this agent.
    /// </summary>
    public Type AgentType { get; set; }



    //_________________________________________________________________________
    // Methods for own query interface and property setters.     


    /// <summary>
    ///   The position of the agent.
    /// </summary>
    public Vector3 Position {
      get { return Shape.Position; }
      //TODO SET
    }


    /// <summary>
    ///   Dimension of this agent's bounding box.
    /// </summary>
    public Vector3 Dimension {
      get { return Shape.Bounds.Dimension; }
      //TODO SET
    }


    /// <summary>
    ///   The agent's heading.
    /// </summary>
    public Direction Direction {
      get { return Shape.Rotation; }
      //TODO SET
    }


    /// <summary>
    ///   Public readable agent information string for various purposes. 
    /// </summary>
    public string AgentInformation { get; set; }
  }
}