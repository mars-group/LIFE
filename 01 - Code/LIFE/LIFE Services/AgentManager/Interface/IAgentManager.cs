using System;
using System.Collections.Generic;
using LCConnector.TransportTypes;
using LifeAPI.Agent;

namespace AgentManager.Interface
{
    public interface IAgentManager {
        Dictionary<Guid,IAgent> GetAgentsByAgentInitConfig(AgentInitConfig agentInit);
    }
}
