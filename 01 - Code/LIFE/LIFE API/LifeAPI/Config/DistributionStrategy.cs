using System;

namespace LifeAPI.Config
{
    /// <summary>
    /// Defines the types of distribution strategies usable in a MARS LIFE simulation.
    /// </summary>
    [Serializable]
    public enum DistributionStrategy
    {
        /// <summary>
        /// No distribution is applied.
        /// </summary>
        NO_DISTRIBUTION,

        /// <summary>
        /// All entities will be evenly distributed across all nodes.
        /// </summary>
        EVEN_DISTRIBUTION,
         
        /// <summary>
        /// Only valid on environmental layers using the ESC!
        /// Will activate replication of environment
        /// </summary>
        ENV_REPLICATION
    }
}
