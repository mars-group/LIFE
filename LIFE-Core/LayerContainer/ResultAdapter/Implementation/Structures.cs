using System.Collections.Generic;

namespace ResultAdapter.Implementation
{
    /// <summary>
    ///   Result output interface describing the methods to be fulfilled by the generated loggers.
    /// </summary>
    public interface IGeneratedLogger
    {
        /// <summary>
        ///   This is the first call from the ResultAdapter: Print all fixed properties.
        /// </summary>
        /// <returns>Meta structure with agent information and immutable properties.</returns>
        AgentMetadataEntry GetMetatableEntry();

        /// <summary>
        ///   Generates a full-state message of this simulation object.
        /// </summary>
        /// <returns>A JSON-compliant output for all agent states.</returns>
        AgentFrame GetKeyFrame();

        /// <summary>
        ///   Creates a delta message that expresses the changes to the last tick.
        /// </summary>
        /// <returns>JSON object containing the changes.</returns>
        AgentFrame GetDeltaFrame();
    }


    /// <summary>
    ///   An entry for the agent metadata table.
    /// </summary>
    public class AgentMetadataEntry
    {
        public string AgentId; //* GUID of this agent.
        public string AgentType; //* Derived class type.
        public string Layer; //* The layer this agent lives on.
        public long CreationTick; //* The tick this agent was created in.
        public long? DeletionTick = null; // Deletion of this agent.
        public object[] StaticPosition; // Fixed agent position (x,y,z).
        public object[] StaticOrientation; // Fixed rotation as (yaw,pitch,roll).
        public IDictionary<string, object> StaticProperties; // List of immutable properties.
    }


    /// <summary>
    ///   Output frame (either full or delta-state output) for an agent and tick.
    /// </summary>
    public class AgentFrame
    {
        public bool IsKeyframe; // Is this a key- [or delta] frame?
        public string AgentId; // Associated agent.
        public long Tick; // Simulation tick.
        public object[] Position; // Agent position.
        public object[] Orientation; // Agent orientation (bearing [, pitch]).
        public IDictionary<string, object> Properties; // Agent property listing.
    }


    /// <summary>
    ///   Configuration directives for logger creation.
    /// </summary>
    internal struct LoggerConfig
    {
        internal string TypeName; // Simulation entity type name.
        internal string FullName; // Fully-qualified name, including namespace and assembly.
        internal int OutputFrequency; // Output frequency.
        internal string SpatialType; // Spatial type specifier [GPS/Grid/2D/3D] or 'null'.
        internal bool IsStationary; // Is the object stationary (position fixed)?
        internal Dictionary<string, bool> Properties; // Properties to output (and static flag).
        internal IEnumerable<string> VisParameters; // Visualization parameters to pass along.
    }
}