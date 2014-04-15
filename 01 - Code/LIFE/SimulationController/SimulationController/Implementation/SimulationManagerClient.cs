
namespace SimulationController.Implementation
{
    using System.Collections.Generic;

    using CommonTypes.DataTypes;

    using Hik.Communication.Scs.Communication.EndPoints.Tcp;
    using Hik.Communication.ScsServices.Client;

    using SMConnector;
    using SMConnector.TransportTypes;

    class SimulationManagerClient : ISimulationManager
    {
        private readonly ISimulationManager _simManager;

		private readonly IScsServiceClient<ISimulationManager> _simManagerClient;

        public SimulationManagerClient(NodeInformationType newnode)
        {
			_simManagerClient = ScsServiceClientBuilder.CreateClient<ISimulationManager>(
                new ScsTcpEndPoint(newnode.NodeEndpoint.IpAddress, newnode.NodeEndpoint.Port));

			_simManagerClient.Connect();
            

			_simManager = _simManagerClient.ServiceProxy;


        }

		public void Dispose(){
			_simManagerClient.Dispose ();
			_simManagerClient.Disconnect();
		}

        public ICollection<TModelDescription> GetAllModels() {
            return _simManager.GetAllModels();
        }

        public void StartSimulationWithModel(TModelDescription model, ICollection<NodeInformationType> layerContainers, int? nrOfTicks = null) {
            this._simManager.StartSimulationWithModel(model, layerContainers, nrOfTicks);
        }

        public void PauseSimulation(TModelDescription model) {
            this._simManager.PauseSimulation(model);
        }

        public void ResumeSimulation(TModelDescription model) {
            this._simManager.ResumeSimulation(model);
        }

        public void AbortSimulation(TModelDescription model) {
            this._simManager.AbortSimulation(model);
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            this._simManager.SubscribeForStatusUpdate(statusUpdateAvailable);
        }
    }
}
