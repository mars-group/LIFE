//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 23.01.2016
//  *******************************************************/

using System;
using System.Collections.Generic;
using ASC.Communication.ScsServices.Service;
using LIFE.Components.Services.AgentShadowingService.Interface;


namespace LIFE.Components.Services.AgentShadowingService.Implementation
{
    public class
        AgentShadowingServiceComponent<TServiceInterface, TServiceClass> :
            IAgentShadowingService<TServiceInterface, TServiceClass>
        where TServiceClass : AscService, TServiceInterface
        where TServiceInterface : class
    {
        private readonly IAgentShadowingService<TServiceInterface, TServiceClass> _agentShadowingUseCase;

        public event EventHandler<LIFEAgentEventArgs<TServiceInterface>> AgentUpdates;

        public AgentShadowingServiceComponent(int port = 6666)
        {
            _agentShadowingUseCase = new AgentShadowingServiceUseCase<TServiceInterface, TServiceClass>(port);
            _agentShadowingUseCase.AgentUpdates += OnAgentUpdates;
        }

        private void OnAgentUpdates(object sender, LIFEAgentEventArgs<TServiceInterface> e)
        {
            var handler = AgentUpdates;
            if (handler != null) handler(this, e);
        }


        public TServiceInterface ResolveAgent(Guid agentId)
        {
            return _agentShadowingUseCase.ResolveAgent(agentId);
        }

        public List<TServiceInterface> ResolveAgents(Guid[] agentIds)
        {
            return _agentShadowingUseCase.ResolveAgents(agentIds);
        }

        public void RemoveShadowAgent(Guid agentId)
        {
            _agentShadowingUseCase.RemoveShadowAgent(agentId);
        }

        public void RegisterRealAgent(TServiceClass agentToRegister)
        {
            _agentShadowingUseCase.RegisterRealAgent(agentToRegister);
        }

        public void RegisterRealAgents(TServiceClass[] agentsToRegister)
        {
            _agentShadowingUseCase.RegisterRealAgents(agentsToRegister);
        }

        public void RemoveRealAgent(TServiceClass agentToRemove)
        {
            _agentShadowingUseCase.RemoveRealAgent(agentToRemove);
        }

        public void Dispose()
        {
            _agentShadowingUseCase.Dispose();
        }
    }
}