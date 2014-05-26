using System;
using System.Collections.Generic;
using System.Linq;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using Hik.Collections;
using NodeRegistry.Interface;

namespace NodeRegistry.Implementation.UseCases
{
    class NodeRegistryNodeManagerUseCase {

        private readonly NodeRegistryEventHandlerUseCase _nodeRegistryEventHandlerUseCase;
        
        private readonly ThreadSafeSortedList<String, TNodeInformation> _activeNodeList;


        public NodeRegistryNodeManagerUseCase(NodeRegistryEventHandlerUseCase nodeRegistryEventHandlerUseCase) {
            _nodeRegistryEventHandlerUseCase = nodeRegistryEventHandlerUseCase;
            _activeNodeList = new ThreadSafeSortedList<string, TNodeInformation>();
        }

        public List<TNodeInformation> GetAllNodes()
        {
            return _activeNodeList.GetAllItems();
        }

        public List<TNodeInformation> GetAllNodesByType(NodeType nodeType)
        {
            return GetAllNodes().Where(nodeInformationType => nodeInformationType.NodeType == nodeType).ToList();
        }

        public void AddNode(TNodeInformation nodeInformation) {
            _activeNodeList[nodeInformation.NodeIdentifier] = nodeInformation;

            //notify all subscribers
            _nodeRegistryEventHandlerUseCase.NotifyOnNodeJoinSubsribers(nodeInformation);
            _nodeRegistryEventHandlerUseCase.NotifyOnNodeTypeJoinSubsribers(nodeInformation);

        }

        public void RemoveNode(TNodeInformation nodeInformation) {
            _activeNodeList.Remove(nodeInformation.NodeIdentifier);
       
            //notify leave subsribers
            _nodeRegistryEventHandlerUseCase.NotifyOnNodeLeaveSubsribers(nodeInformation);

        }

        public bool ContainsNode(TNodeInformation nodeInformation) {
            return _activeNodeList.ContainsKey(nodeInformation.NodeIdentifier);
        }


    }
}
