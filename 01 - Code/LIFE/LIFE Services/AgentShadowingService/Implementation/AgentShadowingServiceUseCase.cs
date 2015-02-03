using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentShadowingService.Interface;
using ASC.Communication.ScsServices.Client;
using ASC.Communication.ScsServices.Communication.Messages;
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

        // The dictionary of ShadowAgents, sorted by their Guid
        private readonly IDictionary<Guid, IAscServiceClient<TServiceInterface>> _shadowAgentClients;
        // the Multicast Addresse derived from agent type
        private readonly string _mcastAddress;
        private readonly IAscServiceApplication _agentShadowingServer;
        private readonly int _clientListenPort;
        private readonly LayerContainerSettings _config;

        public AgentShadowingServiceUseCase(int port = 6666) {
            _clientListenPort = port;
            var typeOfTServiceClass = typeof (TServiceClass);
            _shadowAgentClients = new ConcurrentDictionary<Guid, IAscServiceClient<TServiceInterface>>();
            // calculate MulticastAddress for this agentType
            _mcastAddress = MulticastAddressGenerator.GetIPv4MulticastAddressByType(typeOfTServiceClass);
            _agentShadowingServer = AscServiceBuilder.CreateService(port, _mcastAddress);
            _agentShadowingServer.Start();

            // subscribe for remote events
            _agentShadowingServer.AddShadowAgentMessageReceived += OnAddShadowAgentMessageReceived;
            _agentShadowingServer.RemoveShadowAgentMessageReceived += OnRemoveShadowAgentMessageReceived;

            // load config
            _config = Configuration.Load<LayerContainerSettings>();
        }

        private void OnRemoveShadowAgentMessageReceived(object sender, RemoveShadowAgentEventArgs e) {
            RemoveShadowAgent(e.RemoveShadowAgentMessage.AgentID);
            // TODO: trigger event to Layer
        }

        private void OnAddShadowAgentMessageReceived(object sender, AddShadowAgentEventArgs e) {
            CreateShadowAgent(e.AddShadowAgentMessage.AgentID);
            // TODO: trigger event to layer
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

        public List<TServiceInterface> CreateShadowAgents(Guid[] agentIds) {
            var result = new ConcurrentBag<TServiceInterface>();
            Parallel.ForEach(agentIds, id => {
                result.Add(CreateShadowAgent(id));
            });
            return result.ToList();
        } 

        public void RemoveShadowAgent(Guid agentId) {
            _shadowAgentClients[agentId].Disconnect();
            _shadowAgentClients.Remove(agentId);
        }

        public void RegisterRealAgent(TServiceClass agentToRegister)
        {
            _agentShadowingServer.AddService<TServiceInterface, TServiceClass>(agentToRegister);
            // send Message to other hosts to trigger Shadow Agent creation
            _agentShadowingServer.SendMessage(new AddShadowAgentMessage {AgentID = agentToRegister.ServiceID});
        }

        public void RegisterRealAgents(TServiceClass[] agentsToRegister) {
            Parallel.ForEach(agentsToRegister, RegisterRealAgent);
        }

        public void RemoveRealAgent(TServiceClass agentToRemove) {
            _agentShadowingServer.RemoveService<TServiceInterface>(agentToRemove.ServiceID);
            // send message to other hosts to trigger Shadow Agent removal
            _agentShadowingServer.SendMessage(new RemoveShadowAgentMessage {AgentID = agentToRemove.ServiceID});
        }

        public string GetLayerContainerName() {
            return _config.NodeRegistryConfig.NodeIdentifier;
        }
    }
}
