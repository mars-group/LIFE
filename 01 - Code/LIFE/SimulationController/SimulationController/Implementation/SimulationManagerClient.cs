using System;
using System.Collections.Generic;
using CommonTypes.DataTypes;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using SMConnector;
using SMConnector.TransportTypes;

namespace SimulationController.Implementation {

    /// <summary>
    /// The client which actually connects to the SimulationManager
    /// </summary>
    internal class SimulationManagerClient : ISimulationManager {

        private readonly ISimulationManager _simManager;
        private readonly IScsServiceClient<ISimulationManager> _simManagerClient;
        public bool IsConnected { get; private set; }

        public SimulationManagerClient(string ip, int port) {
            IsConnected = false;
            _simManagerClient = ScsServiceClientBuilder.CreateClient<ISimulationManager>(
                new ScsTcpEndPoint(ip, port));

            _simManagerClient.Disconnected += SimManagerClientOnDisconnected;

            _simManagerClient.Connect();
            IsConnected = true;
            _simManager = _simManagerClient.ServiceProxy;
        }

        private void SimManagerClientOnDisconnected(object sender, EventArgs eventArgs) {
            // do nothing for now. maybe later...
        }

        public void Dispose() {
            _simManagerClient.Dispose();
            _simManagerClient.Disconnect();
        }

        public ICollection<TModelDescription> GetAllModels() {
            return _simManager.GetAllModels();
        }

        public void StepSimulation(TModelDescription model, int? nrOfTicks = null) {
            _simManager.StepSimulation(model, nrOfTicks);
        }

        public void StartSimulationWithModel(Guid simulationId, TModelDescription model, int? nrOfTicks = null, bool startPaused = false)
        {
            _simManager.StartSimulationWithModel(simulationId, model, nrOfTicks, startPaused);
        }

        public void PauseSimulation(TModelDescription model) {
            _simManager.PauseSimulation(model);
        }

        public void ResumeSimulation(TModelDescription model) {
            _simManager.ResumeSimulation(model);
        }

        public void AbortSimulation(TModelDescription model) {
            _simManager.AbortSimulation(model);
        }

        public void StartVisualization(TModelDescription model, int? nrOfTicksToVisualize = null) {
            _simManager.StartVisualization(model, nrOfTicksToVisualize);
        }

        public void StopVisualization(TModelDescription model) {
            _simManager.StopVisualization(model);
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            _simManager.SubscribeForStatusUpdate(statusUpdateAvailable);
        }
    }
}