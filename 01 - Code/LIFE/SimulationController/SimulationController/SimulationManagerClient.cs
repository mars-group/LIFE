using System.Collections.Generic;

using CommonTypes.DataTypes;

using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;

namespace SimulationController
{
    using AppSettingsManager;

    using CommonTypes.Types;

    using ConfigurationAdapter.Interface;

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
            IsConnected = false;


            var multiCastAdapter = new MulticastAdapterComponent(Configuration.Load<GlobalConfig>("SimManagerGlobalConfig.cfg"),
                    Configuration.Load<MulticastSenderConfig>("SimManagerMulticastSenderConfig.cfg"));

            _nodeRegistry = new NodeRegistryUseCase(multiCastAdapter, Configuration.Load<NodeRegistryConfig>("SimManagerNodeRegistryConfig.cfg"));

            _nodeRegistry.SubscribeForNewNodeConnectedByType(OnNewSimManagerConnected, NodeType.SimulationManager);


        }

        private void OnNewSimManagerConnected(NodeInformationType newnode) {
            var simManagerClient = ScsServiceClientBuilder.CreateClient<ISimulationManager>(
                new ScsTcpEndPoint(newnode.NodeEndpoint.IpAddress, newnode.NodeEndpoint.Port));

            simManagerClient.Connect();

            _simManager = simManagerClient.ServiceProxy;
            IsConnected = true;

        }

        public IList<TModelDescription> GetAllModels() {
            return this._simManager.GetAllModels();
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
