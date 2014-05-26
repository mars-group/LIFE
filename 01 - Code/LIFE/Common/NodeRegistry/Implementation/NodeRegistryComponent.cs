using System.Collections.Generic;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using MulticastAdapter.Interface;
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



        public NodeRegistryComponent(IMulticastAdapter multicastAdapter, NodeRegistryConfig config) {
            _config = config;

            TNodeInformation locaNodeInformation = ParseNodeInformationTypeFromConfig();

            _eventHandlerUseCase = new NodeRegistryEventHandlerUseCase();
            _nodeManagerUseCase = new NodeRegistryNodeManagerUseCase(_eventHandlerUseCase);
            _heartBeatUseCase = new NodeRegistryHeartBeatUseCase(_nodeManagerUseCase, locaNodeInformation, multicastAdapter, config.HeartBeatInterval, config.HeartBeatTimeOutmultiplier);
            _networkUseCase = new NodeRegistryNetworkUseCase(_nodeManagerUseCase, _heartBeatUseCase, locaNodeInformation, config.AddMySelfToActiveNodeList, multicastAdapter);
          
            _multicastAdapter = multicastAdapter;
          
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

     

        private TNodeInformation ParseNodeInformationTypeFromConfig()
        {
            return new TNodeInformation(
                ParseNodeTypeFromConfig(),
                _config.NodeIdentifier,
                ParseNodeEndpointFromConfig()
                );
        }

        private NodeEndpoint ParseNodeEndpointFromConfig()
        {
            return new NodeEndpoint(_config.NodeEndPointIP, _config.NodeEndPointPort);
        }

        private NodeType ParseNodeTypeFromConfig()
        {
            return _config.NodeType;
        }



    }
}
