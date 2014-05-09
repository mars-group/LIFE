using System.Collections.Generic;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using NodeRegistry.Interface;

namespace SimulationManagerTest.MockComponents {
    internal class NodeRegistryMock : INodeRegistry {
        public List<TNodeInformation> GetAllNodes() {
            throw new System.NotImplementedException();
        }

        public void SubscribeForNewNodeConnected(NewNodeConnected newNodeConnectedHandler) {
            throw new System.NotImplementedException();
        }

        public void LeaveCluster() {
            throw new System.NotImplementedException();
        }

        public void JoinCluster() {
            throw new System.NotImplementedException();
        }

        public void ShutDownNodeRegistry() {
            throw new System.NotImplementedException();
        }

        public void SubscribeForNodeDisconnected(NodeDisconnected nodeDisconnectedHandler, TNodeInformation node) {
            throw new System.NotImplementedException();
        }

        public void SubscribeForNewNodeConnectedByType(NewNodeConnected newNodeConnectedHandler, NodeType nodeType) {
            throw new System.NotImplementedException();
        }

        public List<TNodeInformation> GetAllNodesByType(NodeType nodeType) {
            throw new System.NotImplementedException();
        }
    }
}