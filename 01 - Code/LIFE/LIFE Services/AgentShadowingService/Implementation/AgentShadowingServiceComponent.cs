﻿using System;
using System.Collections.Generic;
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

        public List<TServiceInterface> CreateShadowAgents(Guid[] agentIds) {
            return _agentShadowingUseCase.CreateShadowAgents(agentIds);
        }

        public void RemoveShadowAgent(Guid agentId) {
            _agentShadowingUseCase.RemoveShadowAgent(agentId);
        }

        public void RegisterRealAgent(TServiceClass agentToRegister)
        {
            _agentShadowingUseCase.RegisterRealAgent(agentToRegister);
        }

        public void RegisterRealAgents(TServiceClass[] agentsToRegister) {
            _agentShadowingUseCase.RegisterRealAgents(agentsToRegister);
        }

        public void RemoveRealAgent(TServiceClass agentToRemove) {
            _agentShadowingUseCase.RemoveRealAgent(agentToRemove);
        }

        public string GetLayerContainerName() {
            return _agentShadowingUseCase.GetLayerContainerName();
        }
    }
}
