using System.Collections.Concurrent;
using LIFE.API.Layer.Initialization;

namespace LIFE.Components.Services.AgentManagerService.Implementation
{
    public interface IAgentParameterFetcher
    {
        ConcurrentDictionary<string, string[]> GetParametersForInitConfig(AgentInitConfig agentInitConfig);
    }
}