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


  /// <summary>
  ///   Configuration directives for logger creation.
  /// </summary>
  internal struct LoggerConfig {
    internal string TypeName;                     // ITickClient type name.
    internal int OutputFrequency;                 // Output frequency.
    internal bool IsSpatial;                      // Is the spatial output desired?
    internal bool IsStationary;                   // Is the object stationary (position fixed)?
    internal Dictionary<string, bool> Properties; // Properties to output (and static flag).
    internal IEnumerable<string> VisParameters;   // Visualization parameters to pass along.
  }


  /// <summary>
  ///   Logger function code snippets.
  /// </summary>
  internal struct LoggerCodeFragment {
    internal string TypeName;       // Name of the ITickClient class this logger belongs to.
    internal string MetaCode;       // Code for meta table entry creation.
    internal string KeyframeCode;   // Code for key frame (full state) output.
    internal string DeltaframeCode; // Code for delta frame (changes) output.
  }
}
