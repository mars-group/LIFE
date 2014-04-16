using System.Collections.Generic;
using CommonTypes.DataTypes;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using SMConnector;
using SMConnector.TransportTypes;

namespace SimulationController.Implementation {
    internal class SimulationManagerClient : ISimulationManager {
        private readonly ISimulationManager _simManager;

        private readonly IScsServiceClient<ISimulationManager> _simManagerClient;

        public SimulationManagerClient(NodeInformationType newnode) {
            _simManagerClient = ScsServiceClientBuilder.CreateClient<ISimulationManager>(
                new ScsTcpEndPoint(newnode.NodeEndpoint.IpAddress, newnode.NodeEndpoint.Port));

            _simManagerClient.Connect();


            _simManager = _simManagerClient.ServiceProxy;
        }

        public void Dispose() {
            _simManagerClient.Dispose();
            _simManagerClient.Disconnect();
        }

        public ICollection<TModelDescription> GetAllModels() {
            return _simManager.GetAllModels();
        }

        public void StartSimulationWithModel(TModelDescription model, ICollection<NodeInformationType> layerContainers,
            int? nrOfTicks = null) {
            _simManager.StartSimulationWithModel(model, layerContainers, nrOfTicks);
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

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            _simManager.SubscribeForStatusUpdate(statusUpdateAvailable);
        }
    }
}