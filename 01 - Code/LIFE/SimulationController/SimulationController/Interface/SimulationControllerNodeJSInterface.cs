using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SMConnector;

namespace SimulationController
{
    using AppSettingsManager;

    using CommonTypes.DataTypes;
    using CommonTypes.Types;

    using ConfigurationAdapter.Interface;

    using MulticastAdapter.Implementation;
    using MulticastAdapter.Interface.Config;

    using NodeRegistry.Implementation;
    using NodeRegistry.Interface;

    using SMConnector.TransportTypes;
    using System.Threading.Tasks;

    /// <summary>
    /// This class provides access to the SimulationManager for Egde.js.
    /// It therefore has the suffix 'Interface'
    /// Each method in here has to be of type 'async Task<object>' and
    /// must use object or dynamic parameter types
    /// </summary>
    public class SimulationControllerNodeJsInterface
    {
        private SimulationManagerClient _simulationManagerClient;

        private readonly INodeRegistry _nodeRegistry;

        private bool _isConnected;

        public SimulationControllerNodeJsInterface() {
            _isConnected = false;

            var multiCastAdapter = new MulticastAdapterComponent(Configuration.Load<GlobalConfig>("SimControllerGlobalConfig.cfg"),
                    Configuration.Load<MulticastSenderConfig>("SimControllerMulticastSenderConfig.cfg"));

            var config = Configuration.Load<SimControllerConfig>("SimControllerConfig.cfg");

            _nodeRegistry = new NodeRegistryUseCase(multiCastAdapter, config.NodeRegistryConfig);

            _nodeRegistry.SubscribeForNewNodeConnectedByType(OnNewSimManagerConnected, NodeType.SimulationManager);
        }

        private void OnNewSimManagerConnected(NodeInformationType newnode) {
            _simulationManagerClient = new SimulationManagerClient(newnode);
            _isConnected = true;
        }

        #region ISimulationManager Methods
        public async Task<object> GetAllModels(dynamic input) {
            if (!_isConnected) { return new object[0]; }
            return await Task.Run(
                () => _simulationManagerClient.GetAllModels().ToArray());
        }

        public async Task<object> StartSimulationWithModel(dynamic input)
        {
            if (!_isConnected) { return new object[0]; }
            return await Task.Run(
                () =>
                {
                    _simulationManagerClient.StartSimulationWithModel(new TModelDescription(input.Name),new List<NodeInformationType>());
                    return 0;
                });
        }

        public async Task<object> SubscribeForStatusUpdate(dynamic input)
        {
            if (!_isConnected) { return new object[0]; }
            return await Task.Run(
                () =>
                {
                    _simulationManagerClient.SubscribeForStatusUpdate(OnStatusUpdateAvailable);  
                    return 0;
                });
        }

        public async Task<object> PauseSimulation(dynamic input) {
            if (!_isConnected) { return new object[0]; }
            return await Task.Run(() => this._simulationManagerClient.PauseSimulation(input));
        }

        public async Task<object> ResumeSimulation(dynamic input)
        {
            if (!_isConnected) { return new object[0]; }
            return await Task.Run(() => this._simulationManagerClient.ResumeSimulation(input));
        }

        public async Task<object> AbortSimulation(dynamic input)
        {
            if (!_isConnected) { return new object[0]; }
            return await Task.Run(() => this._simulationManagerClient.AbortSimulation(input));
        }

        #endregion

        #region StatusMethods

        public async Task<object> GetConnectedNodes(dynamic input) {
            return await Task.Run(
                () => this._nodeRegistry.GetAllNodes());
        }

        #endregion

        private void OnStatusUpdateAvailable(TStatusUpdate update) {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// mock implementaion
    /// </summary>
    public class SimulationManagerClientMock : ISimulationManager {
        public SimulationManagerClientMock(NodeInformationType newnode) {

        }

        public SimulationManagerClientMock() {
        }

        public bool IsConnected {
            get {
                return true;
            } 
        }

        public IList<TModelDescription> GetAllModels() {
            return new TModelDescription[] {
                new TModelDescription("Abdoulaye"),
                new TModelDescription("Cheetahz"), 
                new TModelDescription("Ökonomiezeugs"), 
            };
        }

        public void StartSimulationWithModel(TModelDescription model, ICollection<NodeInformationType> layerContainers, int? nrOfTicks = null) {

            Thread.Sleep(2500);
        }

        public void PauseSimulation(TModelDescription model) {
            throw new NotImplementedException();
        }

        public void ResumeSimulation(TModelDescription model) {
            throw new NotImplementedException();
        }

        public void AbortSimulation(TModelDescription model) {
            throw new NotImplementedException();
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            //throw new System.NotImplementedException();
        }
    }
}
