using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using Hik.Collections;

namespace NodeRegistry.Implementation.UseCases
{
    class NodeRegistryNodeManagerUseCase {

        private NodeRegistryEventHandlerUseCase _nodeRegistryEventHandlerUseCase;
        
        private ThreadSafeSortedList<String, NodeInformationType> _activeNodeList;


        public NodeRegistryNodeManagerUseCase(NodeRegistryEventHandlerUseCase nodeRegistryEventHandlerUseCase) {
            _nodeRegistryEventHandlerUseCase = nodeRegistryEventHandlerUseCase;
            _activeNodeList = new ThreadSafeSortedList<string, NodeInformationType>();
        }

        public List<NodeInformationType> GetAllNodes()
        {
            return _activeNodeList.GetAllItems();
        }

        public List<NodeInformationType> GetAllNodesByType(NodeType nodeType)
        {
            return GetAllNodes().Where(nodeInformationType => nodeInformationType.NodeType == nodeType).ToList();
        }

        public void AddNode(NodeInformationType nodeInformation) {
            _activeNodeList[nodeInformation.NodeIdentifier] = nodeInformation;

            //notify all subsribers
            _nodeRegistryEventHandlerUseCase.NotifyOnNodeJoinSubsribers(nodeInformation);
            _nodeRegistryEventHandlerUseCase.NotifyOnNodeTypeJoinSubsribers(nodeInformation);

        }

        public void RemoveNode(NodeInformationType nodeInformation) {
            _activeNodeList.Remove(nodeInformation.NodeIdentifier);
       
            //notify leave subsribers
            _nodeRegistryEventHandlerUseCase.NotifyOnNodeLeaveSubsribers(nodeInformation);

        }

        public bool ContainsNode(NodeInformationType nodeInformation) {
            return _activeNodeList.ContainsKey(nodeInformation.NodeIdentifier);
        }


    }
}
