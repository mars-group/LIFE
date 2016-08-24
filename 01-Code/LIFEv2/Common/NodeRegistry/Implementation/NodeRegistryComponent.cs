//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using ConfigurationAdapter;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using LIFEUtilities.MulticastAddressGenerator;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Config;
using NodeRegistry.Implementation.UseCases;
using NodeRegistry.Interface;

namespace NodeRegistry.Implementation
{
    public class NodeRegistryComponent : INodeRegistry {

        private readonly NodeRegistryEventHandlerUseCase _eventHandlerUseCase;
        private readonly NodeRegistryHeartBeatUseCase _heartBeatUseCase;
        private readonly NodeRegistryNetworkUseCase _networkUseCase;
        private readonly NodeRegistryNodeManagerUseCase _nodeManagerUseCase;
        private readonly IMulticastAdapter _multicastAdapter;
        private readonly NodeRegistryConfig _config;
        private readonly string _clusterName;


        public event EventHandler<TNodeInformation> SimulationManagerConnected;

        public NodeRegistryComponent(IMulticastAdapter multicastAdapter, NodeRegistryConfig config, string clusterName) {
            _config = config;

            _clusterName = clusterName;

			if (!String.IsNullOrEmpty(clusterName)){
				// recreate MultiCastAdapter with specific mcastGroup if clusterName is set
				var globalSettings = new GlobalConfig();
				globalSettings.MulticastGroupIp = MulticastAddressGenerator.GetIPv4MulticastAddress(clusterName);
				multicastAdapter = new MulticastAdapterComponent(globalSettings, new MulticastSenderConfig());
			}

            var locaNodeInformation = new TNodeInformation(
                _config.NodeType,
                _config.NodeIdentifier,
                new NodeEndpoint(_config.NodeEndPointIP, _config.NodeEndPointPort)
                );

            _eventHandlerUseCase = new NodeRegistryEventHandlerUseCase();
            _eventHandlerUseCase.SimulationManagerConnected += EventHandlerUseCaseOnSimulationManagerConnected;
            _nodeManagerUseCase = new NodeRegistryNodeManagerUseCase(_eventHandlerUseCase);
            _heartBeatUseCase = new NodeRegistryHeartBeatUseCase(_nodeManagerUseCase, locaNodeInformation, multicastAdapter, config.HeartBeatInterval, clusterName, config.HeartBeatTimeOutmultiplier);
            _networkUseCase = new NodeRegistryNetworkUseCase(_nodeManagerUseCase, _heartBeatUseCase, locaNodeInformation, config.AddMySelfToActiveNodeList, multicastAdapter, clusterName);
          
            _multicastAdapter = multicastAdapter;
          
        }

        private void EventHandlerUseCaseOnSimulationManagerConnected(object sender, TNodeInformation nodeInformation)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<TNodeInformation> handler = this.SimulationManagerConnected;

            // Event will be null if there are no subscribers
            if (handler != null) handler(this, nodeInformation);
        }


        public List<TNodeInformation> GetAllNodes() {
            return _nodeManagerUseCase.GetAllNodes();
        }

        public List<TNodeInformation> GetAllNodesByType(NodeType nodeType) {
            return _nodeManagerUseCase.GetAllNodesByType(nodeType);
        }

        public void SubscribeForNewNodeConnected(NewNodeConnected newNodeConnectedHandler) {
            _eventHandlerUseCase.SubscribeForNewNodeConnected(newNodeConnectedHandler);
        }

        public void SubscribeForNewNodeConnectedByType(NewNodeConnected newNodeConnectedHandler, NodeType nodeType) {
            _eventHandlerUseCase.SubscribeForNewNodeConnectedByType(newNodeConnectedHandler, nodeType);
        }

        public void SubscribeForNodeDisconnected(NodeDisconnected nodeDisconnectedHandler, TNodeInformation node) {
            _eventHandlerUseCase.SubscribeForNodeDisconnected(nodeDisconnectedHandler, node);
        }

        public void LeaveCluster() {
            _networkUseCase.LeaveCluster();
        }

        public void JoinCluster() {
           _networkUseCase.JoinCluster();
        }

        public void ShutDownNodeRegistry() {
            _networkUseCase.Shutdown();
            _heartBeatUseCase.Shutdow();
            _multicastAdapter.CloseSocket();
        }
    }
}
