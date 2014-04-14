using System;
using System.Linq;
using CommonTypes.Types;

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

        public SimulationManagerClient(NodeInformationType newnode)
        {
            var simManagerClient = ScsServiceClientBuilder.CreateClient<ISimulationManager>(
                new ScsTcpEndPoint(newnode.NodeEndpoint.IpAddress, newnode.NodeEndpoint.Port));

            simManagerClient.Connect();
            

            _simManager = simManagerClient.ServiceProxy;


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
