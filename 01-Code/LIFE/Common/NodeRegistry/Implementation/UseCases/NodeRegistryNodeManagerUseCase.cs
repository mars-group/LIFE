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
using System.Linq;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using CustomUtilities.Collections;


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
            if (!_activeNodeList.ContainsKey(nodeInformation.NodeIdentifier)) return;
            _activeNodeList.Remove(nodeInformation.NodeIdentifier);

            //notify leave subsribers
            _nodeRegistryEventHandlerUseCase.NotifyOnNodeLeaveSubsribers(nodeInformation);
        }

        public bool ContainsNode(TNodeInformation nodeInformation) {
            return _activeNodeList.ContainsKey(nodeInformation.NodeIdentifier);
        }


    }
}
