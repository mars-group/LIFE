using System;
using AgentShadowingService.Interface;

namespace AgentShadowingService.Implementation
{
    internal class AgentShadowingServiceUseCase : IAgentShadowingService
    {
        public T CreateShadowAgent<T>(Guid agentId) where T : class
        {
            throw new NotImplementedException();
        }

        public void RegisterRealAgent()
        {
            throw new NotImplementedException();
        }
    }
}
