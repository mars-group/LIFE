using System;
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

namespace SimulationController.Implementation {
    public class SimulationControllerUseCase {
        private readonly INodeRegistry _nodeRegistry;

        private Dictionary<Guid, SimulationManagerClient> _simulationManagerClients;

        public SimulationControllerUseCase() {
            _simulationManagerClients = new Dictionary<Guid, SimulationManagerClient>();

            /*
             * Not needed atm, since we connect directly via ip and port
             * 
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
            */
        }

        #region ISimulationManager delegates

        public void SetupNewSimulationRun(Guid simulationId, string ip, int port) {
            _simulationManagerClients[simulationId] = new SimulationManagerClient(ip, port);
        }

        public void StartSimulationWithModel
            (Guid simulationId, TModelDescription model, bool startPaused = false, int? nrOfTicks = null) {
                _simulationManagerClients[simulationId].StartSimulationWithModel(model, nrOfTicks, startPaused);
        }

        public void StepSimulation(Guid simulationId, TModelDescription model,  int? nrOfTicks = null)
        {
            if (!_simulationManagerClients[simulationId].IsConnected) throw new NoSimulationManagerConnectedException();
            _simulationManagerClients[simulationId].StepSimulation(model, nrOfTicks);
        }

        public void PauseSimulation(Guid simulationId, TModelDescription model)
        {
            if (!_simulationManagerClients[simulationId].IsConnected) throw new NoSimulationManagerConnectedException();
            _simulationManagerClients[simulationId].PauseSimulation(model);
        }

        public void ResumeSimulation(Guid simulationId, TModelDescription model)
        {
            if (!_simulationManagerClients[simulationId].IsConnected) throw new NoSimulationManagerConnectedException();
            _simulationManagerClients[simulationId].ResumeSimulation(model);
        }

        public void AbortSimulation(Guid simulationId, TModelDescription model)
        {
            if (!_simulationManagerClients[simulationId].IsConnected) throw new NoSimulationManagerConnectedException();
            _simulationManagerClients[simulationId].AbortSimulation(model);
        }

        public void StartVisualization(Guid simulationId, TModelDescription model, int? nrOfTicksToVisualize = null)
        {
            if (!_simulationManagerClients[simulationId].IsConnected) throw new NoSimulationManagerConnectedException();
            _simulationManagerClients[simulationId].StartVisualization(model, nrOfTicksToVisualize);
        }

        public void StopVisualization(Guid simulationId, TModelDescription model)
        {
            if (!_simulationManagerClients[simulationId].IsConnected) throw new NoSimulationManagerConnectedException();
            _simulationManagerClients[simulationId].StopVisualization(model);
        }

        public void SubscribeForStatusUpdate(Guid simulationId, StatusUpdateAvailable statusUpdateAvailable)
        {
            if (!_simulationManagerClients[simulationId].IsConnected) throw new NoSimulationManagerConnectedException();
            _simulationManagerClients[simulationId].SubscribeForStatusUpdate(statusUpdateAvailable);
        }

        #endregion

        #region INodeRegistry delegates

        public List<TNodeInformation> GetConnectedNodes() {
            return _nodeRegistry.GetAllNodes();
        }

        #endregion
    }
}