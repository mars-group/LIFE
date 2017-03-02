//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 25.01.2016
//  *******************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ASC.Communication.ScsServices.Client;
using ASC.Communication.ScsServices.Communication.Messages;
using ASC.Communication.ScsServices.Service;
using System.Threading.Tasks;
using LIFE.Components.Utilities.MulticastAddressGenerator;
using LIFE.Services.AgentShadowingService.Interface;

namespace LIFE.Services.AgentShadowingService.Implementation
{
    internal class AgentShadowingServiceUseCase<TServiceInterface, TServiceClass> : IAgentShadowingService<TServiceInterface, TServiceClass>
        where TServiceClass : AscService, TServiceInterface
        where TServiceInterface : class
    {

        // The dictionary of ShadowAgents, sorted by their Guid
        private readonly ConcurrentDictionary<Guid, IAscServiceClient<TServiceInterface>> _shadowAgentClients;

        // the Multicast Address derived from agent type
        private readonly string _mcastAddress;
        private readonly IAscServiceApplication _agentShadowingServer;
        private readonly int _clientListenPort;
		private readonly string _typeOfServiceClassName;
		private readonly string _typeOfServiceInterfaceName;

        private readonly object _syncRoot = new Object();
        private readonly Type _typeOfTServiceInterface;

        public event EventHandler<LIFEAgentEventArgs<TServiceInterface>> AgentUpdates;

        public AgentShadowingServiceUseCase(int port = 6666) {
            _clientListenPort = port;
            var typeOfTServiceClass = typeof (TServiceClass);
			_typeOfServiceClassName = typeOfTServiceClass.Name;
            _typeOfTServiceInterface = typeof (TServiceInterface);

            _typeOfServiceInterfaceName = _typeOfTServiceInterface.Name;
			_shadowAgentClients = new ConcurrentDictionary<Guid, IAscServiceClient<TServiceInterface>>();

            // calculate MulticastAddress for this agentType
            _mcastAddress = MulticastAddressGenerator.GetIPv4MulticastAddress(typeOfTServiceClass);
			_agentShadowingServer = AscServiceFactory.CreateService(port, _mcastAddress, typeOfTServiceClass.Name);
            _agentShadowingServer.Start();

            // subscribe for remote events
            _agentShadowingServer.AddShadowAgentMessageReceived += OnAddShadowAgentMessageReceived;
            _agentShadowingServer.RemoveShadowAgentMessageReceived += OnRemoveShadowAgentMessageReceived;
        }

        private void OnRemoveShadowAgentMessageReceived(object sender, RemoveShadowAgentEventArgs e) {
            var agentId = e.RemoveShadowAgentMessage.AgentID;
            // break if we don't have that agent
            if (!_shadowAgentClients.ContainsKey(agentId)) return;
            var handler = AgentUpdates;
            if(handler!=null) handler(this, new LIFEAgentEventArgs<TServiceInterface>(
                new List<TServiceInterface>{_shadowAgentClients[e.RemoveShadowAgentMessage.AgentID].ServiceProxy},
                new List<TServiceInterface>()
                ));
            // TODO: Collect agents to remove and remove them after tick (that is: layer controlled)
            RemoveShadowAgent(e.RemoveShadowAgentMessage.AgentID);
        }

        private void OnAddShadowAgentMessageReceived(object sender, AddShadowAgentEventArgs e) {
            var agentId = e.AddShadowAgentMessage.AgentID;
            // break if we already have that agent
            if (_shadowAgentClients.ContainsKey(agentId)) return;

            var handler = AgentUpdates;
            if (handler != null) handler(this, new LIFEAgentEventArgs<TServiceInterface>(
                   // remove list is empty
                   new List<TServiceInterface>(),
                   // add list contains new agent
                   new List<TServiceInterface> {
                       ResolveAgent(agentId)
                   }
            ));
        }



		public TServiceInterface ResolveAgent(Guid agentId)
        {
			// first check whether this agent lives locally on this node
		    var serviceapp = AscServiceFactory.GetServiceApplicationByTypeName (_typeOfServiceClassName);
			if (serviceapp != null && serviceapp.ContainsService<TServiceInterface, TServiceClass> (agentId, _typeOfServiceInterfaceName)) {
				return serviceapp.GetServiceByID<TServiceInterface, TServiceClass> (agentId, _typeOfServiceInterfaceName);
			}

			// agent is not on local node, so create ShadowAgent

            if (_shadowAgentClients.ContainsKey(agentId))
            {
                return _shadowAgentClients[agentId].ServiceProxy;
            }

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
            _shadowAgentClients.TryAdd(agentId, shadowAgentClient);
            // return RealProxy interface wrapper as clientside reference to remote object
            return shadowAgentClient.ServiceProxy;
        

        }

		public List<TServiceInterface> ResolveAgents(Guid[] agentIds)
        {
			return agentIds.AsParallel().Select(ResolveAgent).ToList();
        }

        public void RemoveShadowAgent(Guid agentId)
        {
            lock (_syncRoot)
            {
                _shadowAgentClients[agentId].Disconnect();
                IAscServiceClient<TServiceInterface> bla;
                _shadowAgentClients.TryRemove(agentId, out bla);
            }

        }

        public void RegisterRealAgent(TServiceClass agentToRegister)
        {
            _agentShadowingServer.AddService<TServiceInterface, TServiceClass>(agentToRegister, _typeOfTServiceInterface);
            // send Message to other hosts to trigger Shadow Agent creation
            //_agentShadowingServer.SendMessage(new AddShadowAgentMessage {AgentID = agentToRegister.ServiceID});
        }

        public void RegisterRealAgents(TServiceClass[] agentsToRegister) {
			Parallel.ForEach (agentsToRegister, RegisterRealAgent);
        }

        public void RemoveRealAgent(TServiceClass agentToRemove) {
            _agentShadowingServer.RemoveService<TServiceInterface>(agentToRemove.ServiceID);
            // send message to other hosts to trigger Shadow Agent removal
            //_agentShadowingServer.SendMessage(new RemoveShadowAgentMessage {AgentID = agentToRemove.ServiceID});
        }

        public void Dispose()
        {
            _agentShadowingServer.Stop();
            _shadowAgentClients.Clear();
        }
    }
}
