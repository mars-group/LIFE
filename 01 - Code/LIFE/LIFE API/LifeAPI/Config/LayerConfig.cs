using System;
using System.Collections.Generic;

namespace LifeAPI.Config
{
    [Serializable]
    public class LayerConfig
    {
        public string LayerName { get; set; }

        public List<AgentConfig> AgentConfigs { get; set; }

        public DistributionStrategy DistributionStrategy { get; set; }

        public LayerConfig() {
            DistributionStrategy = DistributionStrategy.NO_DISTRIBUTION;
            AgentConfigs = new List<AgentConfig>();
            LayerName = "Noname";
        }

        public LayerConfig(string layerName, DistributionStrategy distributionStrategy, List<AgentConfig> agentConfigs) {
            DistributionStrategy = distributionStrategy;
            AgentConfigs = agentConfigs;
            LayerName = layerName;
        }
    }
}
