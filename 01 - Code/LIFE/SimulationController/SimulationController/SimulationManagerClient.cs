using System.Collections;
using System.Collections.Generic;
using CommonTypes.DataTypes;
using CommonTypes.TransportTypes;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Config.Types;

namespace SimulationController
{
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
        private ISimulationManager _simManager;

        private INodeRegistry _nodeRegistry;

        public bool IsConnected { get; private set; }

        public SimulationManagerClient() {

            var multiCastAdapter = new MulticastAdapterComponent();

            _nodeRegistry = new NodeRegistryComponent(multiCastAdapter);

            IsConnected = false;

            // wait for SimManager to come up.
            _nodeRegistry.SubscribeForNewNodeConnectedByType(OnNewSimManagerConnected, NodeType.SimulationManager);
        }

        /// <summary>
        /// Wait handle for SimManager connecting. Once a SimManager is connected
        /// this method will continue intialization and execution
        /// </summary>
        /// <param name="newnode"></param>
        private void OnNewSimManagerConnected(NodeInformationType newnode) {
           
            var simManagerNode = newnode;
           
            var simManagerClient = ScsServiceClientBuilder.CreateClient<ISimulationManager>(
                new ScsTcpEndPoint(simManagerNode.NodeEndpoint.IpAddress, simManagerNode.NodeEndpoint.Port));

            simManagerClient.Connect();

            _simManager = simManagerClient.ServiceProxy;
            IsConnected = true;
        }

        public IList<TModelDescription> GetAllModels() {
            return _simManager.GetAllModels();
        }

        public void StartSimulationWithModel(TModelDescription model) {
            _simManager.StartSimulationWithModel(model);
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
