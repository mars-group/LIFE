using System.Collections.Generic;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using ConfigurationAdapter.Interface;
using MulticastAdapter.Interface;
using NodeRegistry.Interface;

namespace NodeRegistry.Implementation
{
    public class NodeRegistryComponent : INodeRegistry
    {
        private readonly INodeRegistry _nodeRegistryUseCase;

        public NodeRegistryComponent(IMulticastAdapter multicastAdapter, NodeRegistryConfig config) {
            _nodeRegistryUseCase = new NodeRegistryUseCase(multicastAdapter, config);
        }

        public List<NodeInformationType> GetAllNodes() {
            return _nodeRegistryUseCase.GetAllNodes();
        }

        public List<NodeInformationType> GetAllNodesByType(NodeType nodeType) {
            return _nodeRegistryUseCase.GetAllNodesByType(nodeType);
        }

        public void SubscribeForNewNodeConnected(NewNodeConnected newNodeConnectedHandler) {
            _nodeRegistryUseCase.SubscribeForNewNodeConnected(newNodeConnectedHandler);
        }

        public void SubscribeForNewNodeConnectedByType(NewNodeConnected newNodeConnectedHandler, NodeType nodeType) {
            _nodeRegistryUseCase.SubscribeForNewNodeConnectedByType(newNodeConnectedHandler, nodeType);
        }

        public void LeaveCluster() {
            _nodeRegistryUseCase.LeaveCluster();
        }

        public void JoinCluster() {
            _nodeRegistryUseCase.JoinCluster();
        }

        public void ShutDownNodeRegistry() {
            _nodeRegistryUseCase.ShutDownNodeRegistry();
        }

        public NodeRegistryConfig GetConfig() {
            return _nodeRegistryUseCase.GetConfig();
        }
    }
}
