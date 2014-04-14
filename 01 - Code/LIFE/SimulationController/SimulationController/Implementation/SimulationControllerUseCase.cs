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
        private ISimulationManager _simulationManagerClient;

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

            NodeInformationType simManagerNode = null;
            while (simManagerNode == null) {
                simManagerNode = _nodeRegistry.GetAllNodesByType(NodeType.SimulationManager).FirstOrDefault();

            }
            _isConnected = true;
            _simulationManagerClient = new SimulationManagerClient(simManagerNode);
            /*else {
                _nodeRegistry.SubscribeForNewNodeConnectedByType(
                    OnNewSimManagerConnected,
                    NodeType.SimulationManager);
            }
             * */
        }

        private void OnNewSimManagerConnected(NodeInformationType newnode) {
            _simulationManagerClient = new SimulationManagerClient(newnode);
            _isConnected = true;
        }

        #region ISimulationManager delegates
        public ICollection<TModelDescription> GetAllModels() {
            return this._simulationManagerClient.GetAllModels();
        }

        public void StartSimulationWithModel(TModelDescription model, ICollection<NodeInformationType> layerContainers, int? nrOfTicks = null) {
            this._simulationManagerClient.StartSimulationWithModel(model, layerContainers, nrOfTicks);
        }

        public void PauseSimulation(TModelDescription model) {
            this._simulationManagerClient.PauseSimulation(model);
        }

        public void ResumeSimulation(TModelDescription model) {
            this._simulationManagerClient.ResumeSimulation(model);
        }

        public void AbortSimulation(TModelDescription model) {
            this._simulationManagerClient.AbortSimulation(model);
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
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