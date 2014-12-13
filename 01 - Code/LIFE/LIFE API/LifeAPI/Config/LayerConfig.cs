using System;
using System.Collections.Generic;

namespace LifeAPI.Config
{
    [Serializable]
    public class LayerConfig
    {
        public string LayerName { get; set; }

        public List<AgentConfig> AgentConfigs { get; set; }

        public bool Distributable { get; set; }

        public DistributionStrategy DistributionStrategy { get; set; }

        public LayerConfig(string layerName, bool distributable, DistributionStrategy distributionStrategy, List<AgentConfig> agentConfigs) {
            
            Distributable = distributable;
            DistributionStrategy = distributionStrategy;
            AgentConfigs = agentConfigs;
            LayerName = layerName;
        }
    }
}
