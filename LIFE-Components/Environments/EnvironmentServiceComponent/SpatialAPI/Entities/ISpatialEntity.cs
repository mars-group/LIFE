using System;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.ESC.SpatialAPI.Entities
{
    /// <summary>
    ///   An entity or agent substitute for environment related compatibility.
    /// </summary>
    public interface ISpatialEntity 
    {
        /// <summary>
        ///   The globally unique agent ID associated with this entity.
        /// </summary>
        Guid AgentGuid { get; }

        /// <summary>
        ///   The agent type of the associated agent.
        /// </summary>
        Type AgentType { get; }

        /// <summary>
        ///   Describes the spatial expansion in a defined form.
        /// </summary>
        IShape Shape { get; set; }


        /// <summary>
        ///   Return the information type specified by this object.
        /// </summary>
        Enum CollisionType { get; }
    }
}