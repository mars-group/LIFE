using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using AppSettingsManager;
using CommonTypes.DataTypes;
using CommonTypes.TransportTypes;
using ConfigurationAdapter.Interface;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Config;

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
            IsConnected = false;


            var multiCastAdapter = new MulticastAdapterComponent(Configuration.GetConfiguration<GlobalConfig>("SimManagerGlobalConfig.cfg"),
                    Configuration.GetConfiguration<MulticastSenderConfig>("SimManagerMulticastSenderConfig.cfg"));

            _nodeRegistry = new NodeRegistryUseCase(multiCastAdapter, Configuration.GetConfiguration<NodeRegistryConfig>("SimManagerNodeRegistryConfig.cfg"));

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
