using System.CodeDom;
using System.Collections.Generic;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using Hik.Communication.ScsServices.Communication;
using MulticastAdapter.Interface;
using NodeRegistry.Implementation.UseCases;
using NodeRegistry.Interface;

namespace NodeRegistry.Implementation
{
    public class NodeRegistryComponent : INodeRegistry {

        private NodeRegistryEventHandlerUseCase _eventHandlerUseCase;
        private NodeRegistryHeartBeatUseCase _heartBeatUseCase;
        private NodeRegistryNetworkUseCase _networkUseCase;
        private NodeRegistryNodeManagerUseCase _nodeManagerUseCase;
        private IMulticastAdapter _multicastAdapter;

        private NodeRegistryConfig _config;



        public NodeRegistryComponent(IMulticastAdapter multicastAdapter, NodeRegistryConfig config) {
            _config = config;

            NodeInformationType locaNodeInformation = ParseNodeInformationTypeFromConfig();

            _eventHandlerUseCase = new NodeRegistryEventHandlerUseCase();
            _nodeManagerUseCase = new NodeRegistryNodeManagerUseCase(_eventHandlerUseCase);
            _heartBeatUseCase = new NodeRegistryHeartBeatUseCase(_nodeManagerUseCase, locaNodeInformation, multicastAdapter, config.HeartBeatInterval, config.HeartBeatTimeOutmultiplier);
            _networkUseCase = new NodeRegistryNetworkUseCase(_nodeManagerUseCase, _heartBeatUseCase, locaNodeInformation, config.AddMySelfToActiveNodeList, multicastAdapter);
          
            _multicastAdapter = multicastAdapter;
          
        }

        public List<NodeInformationType> GetAllNodes() {
            return _nodeManagerUseCase.GetAllNodes();
        }

        public List<NodeInformationType> GetAllNodesByType(NodeType nodeType) {
            return _nodeManagerUseCase.GetAllNodesByType(nodeType);
        }

        public void SubscribeForNewNodeConnected(NewNodeConnected newNodeConnectedHandler) {
            _eventHandlerUseCase.SubscribeForNewNodeConnected(newNodeConnectedHandler);
        }

        public void SubscribeForNewNodeConnectedByType(NewNodeConnected newNodeConnectedHandler, NodeType nodeType) {
            _eventHandlerUseCase.SubscribeForNewNodeConnectedByType(newNodeConnectedHandler, nodeType);
        }

        public void SubscribeForNodeDisconnected(NodeDisconnected nodeDisconnectedHandler, NodeInformationType node) {
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

     

        private NodeInformationType ParseNodeInformationTypeFromConfig()
        {
            return new NodeInformationType(
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
