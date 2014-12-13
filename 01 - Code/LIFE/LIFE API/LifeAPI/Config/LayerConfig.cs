using System;

namespace LifeAPI.Config
{
    [Serializable]
    public class LayerConfig
    {
        public string LayerName { get; set; }

        public int TotalAgentCount { get; set; }

        public bool Distributable { get; set; }

        public DistributionStrategy DistributionStrategy { get; set; }

        public LayerConfig(string layerName, int totalAgentCount, bool distributable, DistributionStrategy distributionStrategy) {
            TotalAgentCount = totalAgentCount;
            Distributable = distributable;
            DistributionStrategy = distributionStrategy;
            LayerName = layerName;
        }
    }
}
