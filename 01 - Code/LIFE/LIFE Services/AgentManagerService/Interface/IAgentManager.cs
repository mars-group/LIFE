using System;
using System.Collections.Generic;
using LCConnector.TransportTypes;
using LifeAPI.Agent;
using LifeAPI.Layer;
using SpatialAPI.Environment;

namespace AgentManager.Interface
{
    public interface IAgentManager<T> where T : IAgent {
		IDictionary<Guid,T> GetAgentsByAgentInitConfig(AgentInitConfig agentInitConfig, IEnvironment environment, List<ILayer> additionalLayerDependencies);
    }
}
