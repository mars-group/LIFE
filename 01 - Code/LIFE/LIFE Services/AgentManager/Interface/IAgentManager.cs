using System;
using System.Collections.Generic;
using LCConnector.TransportTypes;
using LifeAPI.Agent;
using LifeAPI.Layer;

namespace AgentManager.Interface
{
    public interface IAgentManager {
		Dictionary<Guid,IAgent> GetAgentsByAgentInitConfig(AgentInitConfig agentInitConfig, List<ILayer> additionalLayerDependencies);
    }
}
