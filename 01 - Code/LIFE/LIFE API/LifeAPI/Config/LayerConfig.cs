using System;
using System.Collections.Generic;

namespace LifeAPI.Config
{
    /// <summary>
    /// A configuration for a layer
    /// </summary>
    [Serializable]
    public class LayerConfig
    {
        /// <summary>
        /// The class name of the layer
        /// </summary>
        public string LayerName { get; set; }

        /// <summary>
        /// The layers agent configs.
        /// </summary>
        public List<AgentConfig> AgentConfigs { get; set; }


        /// <summary>
        /// The chosen DistributionStrategy for this layer.
        /// </summary>
        public DistributionStrategy DistributionStrategy { get; set; }

        /// <summary>
        /// Creates a default LayerConfig without distribution and an empty list of AgentConfigs.
        /// </summary>
        public LayerConfig() {
            DistributionStrategy = DistributionStrategy.NO_DISTRIBUTION;
            AgentConfigs = new List<AgentConfig>();
            LayerName = "Noname";
        }

        /// <summary>
        /// Creates a new LayerConfig
        /// </summary>
        /// <param name="layerName">The layer's class name</param>
        /// <param name="distributionStrategy">The layer's distribution strategy</param>
        /// <param name="agentConfigs">The layer's AgentConfigs</param>
        public LayerConfig(string layerName, DistributionStrategy distributionStrategy, List<AgentConfig> agentConfigs) {
            DistributionStrategy = distributionStrategy;
            AgentConfigs = agentConfigs;
            LayerName = layerName;
        }
    }
}
