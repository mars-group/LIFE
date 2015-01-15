using System.Collections.Generic;
using DMConnector.TransportTypes;
using Hik.Communication.ScsServices.Service;

namespace DMConnector
{
    /// <summary>
    /// The DistributionManager connector.
    /// </summary>
    [ScsService(Version = "0.1")]
    public interface IDMConnector {
        void UpdateShadowAgents(List<TShadowAgent> agentsToAdd, List<TShadowAgent> agentsToRemove);
    }
}
