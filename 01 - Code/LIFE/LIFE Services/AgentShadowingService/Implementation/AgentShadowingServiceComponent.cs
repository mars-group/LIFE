using System;
using AgentShadowingService.Interface;
using ASC.Communication.ScsServices.Service;
using LayerContainerShared;

namespace AgentShadowingService.Implementation
{
    public class AgentShadowingServiceComponent<TServiceInterface, TServiceClass> : IAgentShadowingService<TServiceInterface, TServiceClass>
        where TServiceClass : AscService, TServiceInterface
        where TServiceInterface : class
    {
        private readonly IAgentShadowingService<TServiceInterface, TServiceClass> _agentShadowingUseCase;
        private LayerContainerSettings _config;

        public AgentShadowingServiceComponent(int port = 6666)
        {
            _agentShadowingUseCase = new AgentShadowingServiceUseCase<TServiceInterface, TServiceClass>(port);

        }

        public TServiceInterface CreateShadowAgent(Guid agentId)
        {
            return _agentShadowingUseCase.CreateShadowAgent(agentId);
        }

        public void RemoveShadowAgent(Guid agentId) {
            _agentShadowingUseCase.RemoveShadowAgent(agentId);
        }

        public void RegisterRealAgent(TServiceClass agentToRegister)
        {
            _agentShadowingUseCase.RegisterRealAgent(agentToRegister);
        }

        public void RemoveRealAgent(TServiceClass agentToRemove) {
            _agentShadowingUseCase.RemoveRealAgent(agentToRemove);
        }

        public string GetLayerContainerName() {
            return _agentShadowingUseCase.GetLayerContainerName();
        }
    }
}
