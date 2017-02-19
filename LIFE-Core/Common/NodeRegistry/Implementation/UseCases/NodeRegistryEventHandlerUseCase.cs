//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using CustomUtilities.Collections;
using NodeRegistry.Interface;

namespace NodeRegistry.Implementation.UseCases
{
    public class NodeRegistryEventHandlerUseCase
    {
        public event EventHandler<TNodeInformation> SimulationManagerConnected;

        /// <summary>
        ///     Delegates for different subscriber types.
        /// </summary>
        private NewNodeConnected _newNodeConnectedHandler;

        private NewNodeConnected _newLayerContainerConnectedHandler;
        private NewNodeConnected _newSimulationManagerConnectedHandler;
        private NewNodeConnected _newSimulationControllerConnectedHandler;

        private readonly ThreadSafeSortedList<TNodeInformation, NodeDisconnected> _disconnectedNodeEventHandlers;

        public NodeRegistryEventHandlerUseCase() {
            _disconnectedNodeEventHandlers = new ThreadSafeSortedList<TNodeInformation, NodeDisconnected>();
        }


        public void SubscribeForNodeDisconnected(NodeDisconnected nodeDisconnectedHandler, TNodeInformation node) {
            _disconnectedNodeEventHandlers[node] += nodeDisconnectedHandler;
        }

        public void SubscribeForNewNodeConnected(NewNodeConnected newNodeConnectedHandler)
        {
            _newNodeConnectedHandler += newNodeConnectedHandler;
        }

        public void SubscribeForNewNodeConnectedByType(NewNodeConnected newNodeConnectedHandler, NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.LayerContainer:
                    _newLayerContainerConnectedHandler += newNodeConnectedHandler;
                    break;
                case NodeType.SimulationController:
                    _newSimulationControllerConnectedHandler += newNodeConnectedHandler;
                    break;
                case NodeType.SimulationManager:
                    _newSimulationManagerConnectedHandler += newNodeConnectedHandler;
                    break;
            }
        }

        public void NotifyOnNodeJoinSubsribers(TNodeInformation nodeInformation)
        {
            if (_newNodeConnectedHandler != null) _newNodeConnectedHandler.Invoke(nodeInformation);
        }

        public void NotifyOnNodeTypeJoinSubsribers(TNodeInformation nodeInformation)
        {
            switch (nodeInformation.NodeType)
            {
                case NodeType.LayerContainer:
                    if (_newLayerContainerConnectedHandler != null)
                        _newLayerContainerConnectedHandler.Invoke(nodeInformation);
                    break;
                case NodeType.SimulationController:
                    if (_newSimulationControllerConnectedHandler != null)
                        _newSimulationControllerConnectedHandler.Invoke(nodeInformation);
                    break;
                case NodeType.SimulationManager:
                    if (_newSimulationManagerConnectedHandler != null)
                    {
                        _newSimulationManagerConnectedHandler.Invoke(nodeInformation);
                    }
                    OnSimulationManagerConnected(nodeInformation);
                    break;
            }
        }

        public void NotifyOnNodeLeaveSubsribers(TNodeInformation nodeInformation) {

            if (_disconnectedNodeEventHandlers.ContainsKey(nodeInformation)) {
                if (_disconnectedNodeEventHandlers[nodeInformation] != null) {
                    _disconnectedNodeEventHandlers[nodeInformation].Invoke(nodeInformation);
                } 


            }

        }

        private void OnSimulationManagerConnected(TNodeInformation nodeInformation)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<TNodeInformation> handler = SimulationManagerConnected;

            // Event will be null if there are no subscribers
            if (handler != null) handler(this, nodeInformation);
        }


    }
}
