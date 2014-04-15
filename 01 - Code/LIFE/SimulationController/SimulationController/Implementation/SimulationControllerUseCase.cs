namespace SimulationController.Interface {
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using AppSettingsManager;

    using CommonTypes.DataTypes;
    using CommonTypes.Types;

    using ConfigurationAdapter.Interface;

    using MulticastAdapter.Implementation;
    using MulticastAdapter.Interface.Config;

    using NodeRegistry.Implementation;
    using NodeRegistry.Interface;

    using SimulationController.Implementation;

    using SMConnector;
    using SMConnector.TransportTypes;

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

            _nodeRegistry = new NodeRegistryUseCase(multiCastAdapter, config.NodeRegistryConfig);

			// check if a SimManager is already present, use it, or suspend usage if not present.
			if (!SetupSimManagerNode()) {
                _nodeRegistry.SubscribeForNewNodeConnectedByType(
                    OnNewSimManagerConnected,
                    NodeType.SimulationManager);
            }
            
        }

		private bool SetupSimManagerNode(){
			var simManagerNode = _nodeRegistry.GetAllNodesByType(NodeType.SimulationManager).FirstOrDefault();

			if (simManagerNode != null) {
				_isConnected = true;
				_simulationManagerClient = new SimulationManagerClient(simManagerNode);
				_nodeRegistry.SubscribeForNodeDisconnected (OnNodeDisconnected, simManagerNode);
				return true;
			}
			return false;
		}

        private void OnNewSimManagerConnected(NodeInformationType newnode) {
            _simulationManagerClient = new SimulationManagerClient(newnode);
            _isConnected = true;
        }

		private void OnNodeDisconnected (NodeInformationType oldNode)
		{
			// disconnect and clean up current client
			_simulationManagerClient.Dispose ();

			// try to get new SimManagerNode
			if (!SetupSimManagerNode ()) {
				// didn't work, lets reset state variables :(
				_isConnected = false;
				_simulationManagerClient = null;
			}
		}

        #region ISimulationManager delegates
        public ICollection<TModelDescription> GetAllModels() {
			if (!_isConnected) {
				throw new NoSimulationManagerConnectedException ();
			}
            return this._simulationManagerClient.GetAllModels();
        }

        public void StartSimulationWithModel(TModelDescription model, ICollection<NodeInformationType> layerContainers, int? nrOfTicks = null) {
			if (!_isConnected) {
				throw new NoSimulationManagerConnectedException ();
			}
			this._simulationManagerClient.StartSimulationWithModel(model, layerContainers, nrOfTicks);
        }

        public void PauseSimulation(TModelDescription model) {
			if (!_isConnected) {
				throw new NoSimulationManagerConnectedException ();
			}
			this._simulationManagerClient.PauseSimulation(model);
        }

        public void ResumeSimulation(TModelDescription model) {
			if (!_isConnected) {
				throw new NoSimulationManagerConnectedException ();
			}
			this._simulationManagerClient.ResumeSimulation(model);
        }

        public void AbortSimulation(TModelDescription model) {
			if (!_isConnected) {
				throw new NoSimulationManagerConnectedException ();
			}
			this._simulationManagerClient.AbortSimulation(model);
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
			if (!_isConnected) {
				throw new NoSimulationManagerConnectedException ();
			}
			this._simulationManagerClient.SubscribeForStatusUpdate(statusUpdateAvailable);
        }

        #endregion

        #region INodeRegistry delegates

        public List<NodeInformationType> GetConnectedNodes() {
            return _nodeRegistry.GetAllNodes();
        }

        #endregion
    }
}