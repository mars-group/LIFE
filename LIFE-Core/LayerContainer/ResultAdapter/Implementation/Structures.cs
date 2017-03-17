using System.Collections.Generic;

namespace ResultAdapter.Implementation {


  /// <summary>
  ///   Result output interface describing the methods to be fulfilled by the generated loggers.
  /// </summary>
  public interface IGeneratedLogger {

    /// <summary>
    ///   This is the first call from the ResultAdapter: Print all fixed properties.
    /// </summary>
    /// <returns>Meta structure with agent information and immutable properties.</returns>
    AgentMetadataEntry GetMetatableEntry();

    /// <summary>
    ///   Generates a full-state message of this simulation object.
    /// </summary>
    /// <returns>A JSON-compliant output for all agent states.</returns>
    string GetKeyFrame();

    /// <summary>
    ///   Creates a delta message that expresses the changes to the last tick.
    /// </summary>
    /// <returns>JSON object containing the changes.</returns>
    string GetDeltaFrame();
  }



  /// <summary>
  ///   An entry for the agent metadata table.
  /// </summary>
  public class AgentMetadataEntry {
    public string AgentId;                               //* GUID of this agent.
    public string AgentType;                             //* Derived class type.
    public string Layer;                                 //* The layer this agent lives on.
    public float[] StaticPosition;                       // Fixed agent position (x,y,z).
    public float[] StaticOrientation;                    // Fixed rotation as (yaw,pitch,roll).
    public IDictionary<string, object> StaticProperties; // List of immutable properties.
  }
}
