using System;
using System.Threading;
using SMConnector;

namespace SimulationController
{
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
        private readonly ISimulationManager _simulationManagerClient;

        public SimulationControllerNodeJsInterface() {
            _simulationManagerClient = new SimulationManagerClientMock();//new SimulationManagerClient();
        }

        public async Task<object> GetAllModels(dynamic input)
        {
            return await Task.Run(
                () => _simulationManagerClient.GetAllModels());
        }

        public async Task<object> StartSimulationWithModel(dynamic input)
        {
            return await Task.Run(
                () =>
                {
                    _simulationManagerClient.StartSimulationWithModel(new TModelDescription(input.Name));
                    return 0;
                });
        }

        public async Task<object> SubscribeForStatusUpdate(dynamic input)
        {
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

    public class SimulationManagerClientMock : ISimulationManager {
        public TModelDescription[] GetAllModels() {
            return new TModelDescription[] {
                new TModelDescription("Abdoulaye"),
                new TModelDescription("Cheetahz"), 
                new TModelDescription("Ökonomiezeugs"), 
            };
        }

        public void StartSimulationWithModel(TModelDescription model) {
            
            Thread.Sleep(2500);
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            //throw new System.NotImplementedException();
        }
    }
}
