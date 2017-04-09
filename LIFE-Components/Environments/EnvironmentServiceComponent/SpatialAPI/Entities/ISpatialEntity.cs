using System;

namespace LIFE.Components.ESC.SpatialAPI.Entities
{
    /// <summary>
    ///   An entity or agent substitute for environment related compatibility.
    /// </summary>
    public interface ISpatialEntity : ISpatialObject
    {
        /// <summary>
        ///   The globally unique agent ID associated with this entity.
        /// </summary>
        Guid AgentGuid { get; }

        /// <summary>
        ///   The agent type of the associated agent.
        /// </summary>
        Type AgentType { get; }
    }
}