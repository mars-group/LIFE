using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SMConnector;

namespace SimulationController
{
    using CommonTypes.DataTypes;

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
        private readonly SimulationManagerClient _simulationManagerClient;

        public SimulationControllerNodeJsInterface() {
            _simulationManagerClient = new SimulationManagerClient();//new SimulationManagerClientMock();//
        }

        public async Task<object> GetAllModels(dynamic input) {
            if (!_simulationManagerClient.IsConnected) { return new object[0]; }
            return await Task.Run(
                () => _simulationManagerClient.GetAllModels().ToArray());
        }

        public async Task<object> StartSimulationWithModel(dynamic input)
        {
            if (!_simulationManagerClient.IsConnected) { return new object[0]; }
            return await Task.Run(
                () =>
                {
                    _simulationManagerClient.StartSimulationWithModel(new TModelDescription(input.Name),new List<NodeInformationType>());
                    return 0;
                });
        }

        public async Task<object> SubscribeForStatusUpdate(dynamic input)
        {
            if (!_simulationManagerClient.IsConnected) { return new object[0]; }
            return await Task.Run(
                () =>
                {
                    _simulationManagerClient.SubscribeForStatusUpdate(OnStatusUpdateAvailable);  
                    return 0;
                });
        }

        private void OnStatusUpdateAvailable(TStatusUpdate update) {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// mock implementaion
    /// </summary>
    public class SimulationManagerClientMock : ISimulationManager {
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
