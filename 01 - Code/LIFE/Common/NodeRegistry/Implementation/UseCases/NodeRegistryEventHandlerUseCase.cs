using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using Hik.Collections;
using NodeRegistry.Interface;

namespace NodeRegistry.Implementation.UseCases
{
    public class NodeRegistryEventHandlerUseCase
    {
        

        /// <summary>
        ///     delegates for different subscriber types.
        /// </summary>
        private NewNodeConnected _newNodeConnectedHandler;

        private NewNodeConnected _newLayerContainerConnectedHandler;
        private NewNodeConnected _newSimulationManagerConnectedHandler;
        private NewNodeConnected _newSimulationControllerConnectedHandler;

        private ThreadSafeSortedList<TNodeInformation, NodeDisconnected> _disconnectedNodeEventHandlers;

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
                        _newSimulationManagerConnectedHandler.Invoke(nodeInformation);
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


    }
}
