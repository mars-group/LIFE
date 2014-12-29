using System;
using AgentShadowingService.Interface;
using LifeAPI.Agent;

namespace AgentShadowingService.Implementation
{
    class AgentShadowingServiceComponent : IAgentShadowingService
    {
        private readonly IAgentShadowingService _agentShadowingUseCase;

        public AgentShadowingServiceComponent()
        {
            _agentShadowingUseCase = new AgentShadowingServiceUseCase();
        }

        public T CreateShadowAgent<T>(Guid agentId) where T : class
        {
            return _agentShadowingUseCase.CreateShadowAgent<T>(agentId);
        }

        public void RegisterRealAgent(IAgent agentToRegister)
        {
            _agentShadowingUseCase.RegisterRealAgent(agentToRegister);
        }
    }
}
