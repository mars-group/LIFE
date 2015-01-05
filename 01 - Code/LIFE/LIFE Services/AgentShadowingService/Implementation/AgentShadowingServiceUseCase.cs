﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AgentShadowingService.Interface;
using ASC.Communication.ScsServices.Client;
using ASC.Communication.ScsServices.Service;
using ConfigurationAdapter.Interface;
using LayerContainerShared;
using LIFEUtilities.MulticastAddressGenerator;

namespace AgentShadowingService.Implementation
{
    internal class AgentShadowingServiceUseCase<TServiceInterface, TServiceClass> : IAgentShadowingService<TServiceInterface, TServiceClass>
        where TServiceClass : AscService, TServiceInterface
        where TServiceInterface : class
    {
        private readonly IDictionary<IAscServiceClient<TServiceInterface>, byte> _shadowAgentClients;
        private readonly string _mcastAddress;
        private readonly IScsServiceApplication _agentShadowingServer;
        private int _listenPort;

        public AgentShadowingServiceUseCase(int port = 6666) {
            var typeOfTServiceClass = typeof (TServiceClass);
            _shadowAgentClients = new ConcurrentDictionary<IAscServiceClient<TServiceInterface>, byte>();
            _listenPort = port;
            // calculate MulticastAddress for this agentType
            _mcastAddress = MulticastAddressGenerator.GetIPv4MulticastAddressByType(typeOfTServiceClass);

            // TODO: May be moved into RegisterRealAgent, so as not to start the server, when no real agents are present
            _agentShadowingServer = AscServiceBuilder.CreateService(port, _mcastAddress);
            _agentShadowingServer.Start();
        }

        public TServiceInterface CreateShadowAgent(Guid agentId)
        {
            var shadowAgentClient = AscServiceClientBuilder.CreateClient<TServiceInterface>(
                _listenPort,
                _mcastAddress,
                agentId
                );
            // set timeout to infinite
            shadowAgentClient.Timeout = -1;
            // connect the shadow agent
            shadowAgentClient.Connect();
            // store shadow agent client in list for later management and observation
            _shadowAgentClients.Add(shadowAgentClient, new byte());
            // return RealProxy interface wrapper as clientside reference to remote object
            return shadowAgentClient.ServiceProxy;
        }

        public void RegisterRealAgent(TServiceClass agentToRegister)
        {
            _agentShadowingServer.AddService<TServiceInterface, TServiceClass>(agentToRegister);
        }
    }
}
