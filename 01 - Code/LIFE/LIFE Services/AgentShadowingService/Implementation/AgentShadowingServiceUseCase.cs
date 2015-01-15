using System;
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
        private readonly IDictionary<Guid, IAscServiceClient<TServiceInterface>> _shadowAgentClients;
        private readonly string _mcastAddress;
        private readonly IScsServiceApplication _agentShadowingServer;
        private readonly int _clientListenPort;
        private LayerContainerSettings _config;

        public AgentShadowingServiceUseCase(int port = 6666) {
            _clientListenPort = port;
            var typeOfTServiceClass = typeof (TServiceClass);
            _shadowAgentClients = new ConcurrentDictionary<Guid, IAscServiceClient<TServiceInterface>>();
            // calculate MulticastAddress for this agentType
            _mcastAddress = MulticastAddressGenerator.GetIPv4MulticastAddressByType(typeOfTServiceClass);

            // TODO: May be moved into RegisterRealAgent, so as not to start the server, when no real agents are present
            _agentShadowingServer = AscServiceBuilder.CreateService(port, _mcastAddress);
            _agentShadowingServer.Start();
            _config = Configuration.Load<LayerContainerSettings>();
        }

        public TServiceInterface CreateShadowAgent(Guid agentId)
        {
            var shadowAgentClient = AscServiceClientBuilder.CreateClient<TServiceInterface>(
                _clientListenPort,
                _mcastAddress,
                agentId
                );
            // set timeout to infinite
            shadowAgentClient.Timeout = -1;
            // connect the shadow agent
            shadowAgentClient.Connect();
            // store shadow agent client in list for later management and observation
            _shadowAgentClients.Add(agentId, shadowAgentClient);
            // return RealProxy interface wrapper as clientside reference to remote object
            return shadowAgentClient.ServiceProxy;
        }

        public void RemoveShadowAgent(Guid agentId) {
            _shadowAgentClients[agentId].Disconnect();
            _shadowAgentClients.Remove(agentId);
        }

        public void RegisterRealAgent(TServiceClass agentToRegister)
        {
            _agentShadowingServer.AddService<TServiceInterface, TServiceClass>(agentToRegister);
        }

        public void RemoveRealAgent(TServiceClass agentToRemove) {
            _agentShadowingServer.RemoveService<TServiceInterface>(agentToRemove.ServiceID);
        }

        public string GetLayerContainerName() {
            return _config.NodeRegistryConfig.NodeIdentifier;
        }
    }
}
