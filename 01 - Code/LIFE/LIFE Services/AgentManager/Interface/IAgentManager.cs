using System;
using System.Collections.Generic;
using LCConnector.TransportTypes;
using LifeAPI.Agent;
using LifeAPI.Layer;
using SpatialAPI.Environment;

namespace AgentManager.Interface
{
    public interface IAgentManager {
		Dictionary<Guid,IAgent> GetAgentsByAgentInitConfig(AgentInitConfig agentInitConfig, IEnvironment environment, List<ILayer> additionalLayerDependencies);
    }
}
