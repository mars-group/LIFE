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
        private readonly IDictionary<IAscServiceClient<TServiceInterface>, byte> _shadowAgentClients;
        private readonly string _mcastAddress;
        private readonly IScsServiceApplication _agentShadowingServer;
        private string _address;

        public AgentShadowingServiceUseCase(int port = 6666) {
            var typeOfTServiceClass = typeof (TServiceClass);
            _shadowAgentClients = new ConcurrentDictionary<IAscServiceClient<TServiceInterface>, byte>();

            // calculate MulticastAddress for this agentType
            _mcastAddress = MulticastAddressGenerator.GetIPv4MulticastAddressByType(typeOfTServiceClass);

            // create address string for this node
            var config = Configuration.Load<LayerContainerSettings>();
            _address = "udp://" + config.NodeRegistryConfig.NodeEndPointIP  + ":" + port;

            // TODO: May be moved into RegisterRealAgent, so as not to start the server, when no real agents are present
            _agentShadowingServer = AscServiceBuilder.CreateService(_address, _mcastAddress);
            _agentShadowingServer.Start();
        }

        public TServiceInterface CreateShadowAgent(Guid agentId)
        {
            var shadowAgentClient = AscServiceClientBuilder.CreateClient<TServiceInterface>(
                _address,
                _mcastAddress,
                agentId
                );
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
