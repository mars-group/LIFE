using System;

namespace SpatialAPI.Entities {

    /// <summary>
    /// An entity or agent substitute for environment related compatibility.
    /// </summary>
    public interface ISpatialEntity : ISpatialObject {
        /// <summary>
        ///     Return the information type specified by this object.
        /// </summary>
        Enum CollisionType { get; }

        /// <summary>
        ///     The globally unique agent ID associated with this entity.
        /// </summary>
        Guid AgentGuid { get; }
    }

}