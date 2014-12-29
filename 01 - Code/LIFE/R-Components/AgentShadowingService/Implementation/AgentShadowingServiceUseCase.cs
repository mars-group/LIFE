using System;
using AgentShadowingService.Interface;
using LifeAPI.Agent;

namespace AgentShadowingService.Implementation
{
    internal class AgentShadowingServiceUseCase : IAgentShadowingService
    {
        public T CreateShadowAgent<T>(Guid agentId) where T : class
        {
            throw new NotImplementedException();
        }

        public void RegisterRealAgent(IAgent agentToRegister)
        {
            throw new NotImplementedException();
        }
    }
}
