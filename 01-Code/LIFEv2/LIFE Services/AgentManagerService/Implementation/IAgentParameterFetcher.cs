using System.Collections.Concurrent;
using System.Collections.Generic;
using LCConnector.TransportTypes;

namespace AgentManagerService.Implementation
{
    public interface IAgentParameterFetcher
    {
        ConcurrentDictionary<string, string[]> GetParametersForInitConfig(AgentInitConfig agentInitConfig);
    }
}