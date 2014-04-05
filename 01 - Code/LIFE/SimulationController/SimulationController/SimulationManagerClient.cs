

namespace SimulationController
{
    using System;
    using System.Linq;

    using CommonTypes.Types;

    using Hik.Communication.Scs.Communication.EndPoints.Tcp;
    using Hik.Communication.ScsServices.Client;

    using NodeRegistry.Implementation;
    using NodeRegistry.Interface;

    using SMConnector;
    using SMConnector.TransportTypes;

    class SimulationManagerClient : ISimulationManager
    {

        private readonly ISimulationManager _simManager;

        private INodeRegistry _nodeRegistry;

        public SimulationManagerClient() {
            _nodeRegistry = new NodeRegistryManager();

            _nodeRegistry.StartDiscovery();

            var simManagerNode = _nodeRegistry.GetAllNodesByType(NodeType.SimulationManager).First();

            var simManagerClient = ScsServiceClientBuilder.CreateClient<ISimulationManager>(
                new ScsTcpEndPoint(simManagerNode.NodeEndpoint.IpAddress, simManagerNode.NodeEndpoint.Port));

            simManagerClient.Connect();

            _simManager = simManagerClient.ServiceProxy;
        }

        public TModelDescription[] GetAllModels() {
            return _simManager.GetAllModels();
        }

        public void StartSimulationWithModel(TModelDescription model) {
            _simManager.StartSimulationWithModel(model);
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            _simManager.SubscribeForStatusUpdate(this.OnStatusUpdateAvailable);
        }

        private void OnStatusUpdateAvailable(TStatusUpdate statusUpdate) {
            throw new NotImplementedException();
        }
    }
}
