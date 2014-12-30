using System;
using AgentShadowingService.Interface;
using ASC.Communication.ScsServices.Service;
using LifeAPI.Agent;

namespace AgentShadowingService.Implementation
{
    public class AgentShadowingServiceComponent<TServiceInterface, TServiceClass> : IAgentShadowingService<TServiceInterface, TServiceClass>
        where TServiceClass : AscService, TServiceInterface
        where TServiceInterface : class, IAgent
    {
        private readonly IAgentShadowingService<TServiceInterface, TServiceClass> _agentShadowingUseCase;

        public AgentShadowingServiceComponent()
        {
            _agentShadowingUseCase = new AgentShadowingServiceUseCase<TServiceInterface, TServiceClass>();
        }

        public TServiceInterface CreateShadowAgent(Guid agentId)
        {
            return _agentShadowingUseCase.CreateShadowAgent(agentId);
        }

        public void RegisterRealAgent(TServiceClass agentToRegister)
        {
            _agentShadowingUseCase.RegisterRealAgent(agentToRegister);
        }
    }
}
