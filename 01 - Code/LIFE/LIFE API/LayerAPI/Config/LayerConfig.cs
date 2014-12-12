using System;

namespace LayerAPI.Config
{
    [Serializable]
    public class LayerConfig
    {
        public int TotalAgentCount { get; set; }

        public bool Distributable { get; set; }

        public DistributionStrategy DistributionStrategy { get; set; }

        public LayerConfig(int totalAgentCount, bool distributable, DistributionStrategy distributionStrategy) {
            TotalAgentCount = totalAgentCount;
            Distributable = distributable;
            DistributionStrategy = distributionStrategy;
        }
    }
}
