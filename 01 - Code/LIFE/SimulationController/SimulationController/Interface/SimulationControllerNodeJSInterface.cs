using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

namespace SimulationController.Interface {
    /// <summary>
    ///     This class provides access to the SimulationManager for Egde.js.
    ///     It therefore has the suffix 'Interface'
    ///     Each method in here has to be of type 'async Task
    ///     <object>
    ///         ' and
    ///         must use object or dynamic parameter types
    /// </summary>
    public class SimulationControllerNodeJsInterface {
        private SimulationManagerClient _simulationManagerClient;

        private INodeRegistry _nodeRegistry;

        private bool _isConnected;


        private void Connect() {
            _isConnected = false;
            
            var multiCastAdapter =
                new MulticastAdapterComponent(Configuration.Load<GlobalConfig>("SimControllerGlobalConfig.cfg"),
                    Configuration.Load<MulticastSenderConfig>("SimControllerMulticastSenderConfig.cfg"));

            var config = Configuration.Load<SimControllerConfig>("SimControllerConfig.cfg");

            _nodeRegistry = new NodeRegistryUseCase(multiCastAdapter, config.NodeRegistryConfig);

            var simManagerNode = _nodeRegistry.GetAllNodesByType(NodeType.SimulationManager).FirstOrDefault();
            if (simManagerNode != null) {
                _simulationManagerClient = new SimulationManagerClient(simManagerNode);
                _isConnected = true;
            }
            else _nodeRegistry.SubscribeForNewNodeConnectedByType(OnNewSimManagerConnected, NodeType.SimulationManager);
            

            // MOCK ONLY. REMOVE IF PRODUCTIVE
            //_simulationManagerClient = new SimulationManagerClientMock();
            //_isConnected = _simulationManagerClient.IsConnected;
        }

        private void OnNewSimManagerConnected(NodeInformationType newnode) {
            _simulationManagerClient = new SimulationManagerClient(newnode);
            _isConnected = true;
        }

        #region ISimulationManager Methods

        public async Task<object> GetAllModels(dynamic input) {

            //if (!_isConnected) return new object[0];
             var result = await Task.Run(
                () => {
                    Connect();
                    return _simulationManagerClient.GetAllModels().ToArray();
                });
            return result;
        }

        public async Task<object> StartSimulationWithModel(dynamic input) {
            Connect();
            if (!_isConnected) return new object[0];
            return await Task.Run(
                () => {
                    _simulationManagerClient.StartSimulationWithModel(new TModelDescription(input.Name),
                        new List<NodeInformationType>());
                    return 0;
                });
        }

        public async Task<object> SubscribeForStatusUpdate(dynamic input) {
            if (!_isConnected) return new object[0];
            return await Task.Run(
                () => {
                    _simulationManagerClient.SubscribeForStatusUpdate(OnStatusUpdateAvailable);
                    return 0;
                });
        }

        public async Task<object> PauseSimulation(dynamic input) {
            if (!_isConnected) return new object[0];
            return await Task.Run(
                () => {
                    _simulationManagerClient.PauseSimulation(new TModelDescription(input.Name, input.Description,
                        input.Status.StatusMessage, input.Running));
                    return 0;
                });
        }

        public async Task<object> ResumeSimulation(dynamic input) {
            if (!_isConnected) return new object[0];
            return await Task.Run(
                () => {
                    _simulationManagerClient.ResumeSimulation(new TModelDescription(input.Name, input.Description,
                        input.Status.StatusMessage, input.Running));
                    return 0;
                });
        }

        public async Task<object> AbortSimulation(dynamic input) {
            if (!_isConnected) return new object[0];
            return await Task.Run(
                () => {
                    _simulationManagerClient.AbortSimulation(new TModelDescription(input.Name, input.Description,
                        input.Status.StatusMessage, input.Running));
                    return 0;
                });
        }

        #endregion

        #region StatusMethods

        public async Task<object> GetConnectedNodes(dynamic input) {
            return await Task.Run(
                () => {
                    Connect();
                    return _nodeRegistry.GetAllNodes();
                });
        }

        #endregion

        private void OnStatusUpdateAvailable(TStatusUpdate update) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     mock implementaion
    /// </summary>
    public class SimulationManagerClientMock : ISimulationManager {
        public SimulationManagerClientMock(NodeInformationType newnode) {}

        public SimulationManagerClientMock() {}

        public bool IsConnected {
            get { return true; }
        }

        public ICollection<TModelDescription> GetAllModels() {
            return new[] {
                new TModelDescription("Abdoulaye", "4 Million Trees in Togo"),
                new TModelDescription("Cheetahz", "200 Cheetahs hunting 150.000 Impalas"),
                new TModelDescription("Ökonomiezeugs", "Bewohnersituation in HH")
            };
        }

        public void StartSimulationWithModel(TModelDescription model, ICollection<NodeInformationType> layerContainers,
            int? nrOfTicks = null) {
            Thread.Sleep(100);
        }

        public void PauseSimulation(TModelDescription model) {}

        public void ResumeSimulation(TModelDescription model) {}

        public void AbortSimulation(TModelDescription model) {}

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            //throw new System.NotImplementedException();
        }
    }
}