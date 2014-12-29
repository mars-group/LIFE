using System;
using AgentShadowingService.Interface;
using LifeAPI.Agent;

namespace AgentShadowingService.Implementation
{
    public class AgentShadowingServiceComponent<T> : IAgentShadowingService<T> where T : class, IAgent
    {
        private readonly IAgentShadowingService<T> _agentShadowingUseCase;

        public AgentShadowingServiceComponent()
        {
            _agentShadowingUseCase = new AgentShadowingServiceUseCase<T>();
        }

        public T CreateShadowAgent(Guid agentId)
        {
            return _agentShadowingUseCase.CreateShadowAgent(agentId);
        }

        public void RegisterRealAgent(T agentToRegister)
        {
            _agentShadowingUseCase.RegisterRealAgent(agentToRegister);
        }
    }
}
