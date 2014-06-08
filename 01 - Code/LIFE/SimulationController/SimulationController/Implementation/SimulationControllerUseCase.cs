using System.Collections.Generic;
using System.Linq;
using AppSettingsManager;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using ConfigurationAdapter.Interface;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;
using NodeRegistry.Implementation;
using NodeRegistry.Interface;
using SMConnector;
using SMConnector.TransportTypes;
using System;

namespace SimulationController.Implementation {
    public class SimulationControllerUseCase : ISimulationManager {
        private SimulationManagerClient _simulationManagerClient;

        private readonly INodeRegistry _nodeRegistry;

        private bool _isConnected;

        public SimulationControllerUseCase() {
            _isConnected = false;

            var multiCastAdapter =
                new MulticastAdapterComponent(
                    Configuration.Load<GlobalConfig>("SimControllerGlobalConfig.cfg"),
                    Configuration.Load<MulticastSenderConfig>("SimControllerMulticastSenderConfig.cfg"));

            var config = Configuration.Load<SimControllerConfig>("SimControllerConfig.cfg");

            _nodeRegistry = new NodeRegistryComponent(multiCastAdapter, config.NodeRegistryConfig);

			// check if a Simcontroller is already present, if so, exit
			CheckIfSimControllerIsPresent();

            // check if a SimManager is already present, use it, or suspend usage if not present.
            if (!SetupSimManagerNode()) {
                _nodeRegistry.SubscribeForNewNodeConnectedByType(
                    OnNewSimManagerConnected,
                    NodeType.SimulationManager);
            }
        }

		private void CheckIfSimControllerIsPresent ()
		{
			if (_nodeRegistry.GetAllNodesByType(NodeType.SimulationController).Count > 1) {
				throw new SimControllerAlreadyPresentException ();
			}
		
		}

        private bool SetupSimManagerNode() {
            var simManagerNode = _nodeRegistry.GetAllNodesByType(NodeType.SimulationManager).FirstOrDefault();

            if (simManagerNode != null) {
                _simulationManagerClient = new SimulationManagerClient(simManagerNode);
                _isConnected = true;
                _nodeRegistry.SubscribeForNodeDisconnected(OnNodeDisconnected, simManagerNode);
                return true;
            }
            return false;
        }

        private void OnNewSimManagerConnected(TNodeInformation newnode) {
            _simulationManagerClient = new SimulationManagerClient(newnode);
            _isConnected = true;
        }

        private void OnNodeDisconnected(TNodeInformation oldNode) {
            // disconnect and clean up current client
            _simulationManagerClient.Dispose();

            // try to get new SimManagerNode
            if (!SetupSimManagerNode()) {
                // didn't work, lets reset state variables :(
                _isConnected = false;
                _simulationManagerClient = null;
            }
        }

        #region ISimulationManager delegates

        public ICollection<TModelDescription> GetAllModels() {
            if (!_isConnected) throw new NoSimulationManagerConnectedException();
            return _simulationManagerClient.GetAllModels();
        }

        public void StartSimulationWithModel(TModelDescription model, ICollection<TNodeInformation> layerContainers,
            int? nrOfTicks = null) {
            if (!_isConnected) throw new NoSimulationManagerConnectedException();
            _simulationManagerClient.StartSimulationWithModel(model, layerContainers, nrOfTicks);
        }

        public void PauseSimulation(TModelDescription model) {
            if (!_isConnected) throw new NoSimulationManagerConnectedException();
            _simulationManagerClient.PauseSimulation(model);
        }

        public void ResumeSimulation(TModelDescription model) {
            if (!_isConnected) throw new NoSimulationManagerConnectedException();
            _simulationManagerClient.ResumeSimulation(model);
        }

        public void AbortSimulation(TModelDescription model) {
            if (!_isConnected) throw new NoSimulationManagerConnectedException();
            _simulationManagerClient.AbortSimulation(model);
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            if (!_isConnected) throw new NoSimulationManagerConnectedException();
            _simulationManagerClient.SubscribeForStatusUpdate(statusUpdateAvailable);
        }

        #endregion

        #region INodeRegistry delegates

        public List<TNodeInformation> GetConnectedNodes() {
            return _nodeRegistry.GetAllNodes();
        }

        #endregion
    }
}