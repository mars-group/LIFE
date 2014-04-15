using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using CommonTypes.Types;
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


        public void SubscribeForNodeDisconnected(NodeDisconnected nodeDisconnectedHandler, NodeInformationType node)
        {
            throw new NotImplementedException();
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

        public void NotifyOnNodeJoinSubsribers(NodeInformationType nodeInformation)
        {
            if (_newNodeConnectedHandler != null) _newNodeConnectedHandler.Invoke(nodeInformation);
        }

        public void NotifyOnNodeTypeJoinSubsribers(NodeInformationType nodeInformation)
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

        public void NotifyOnNodeLeaveSubsribers(NodeInformationType nodeInformation) {
           //TODO
        }


    }
}
