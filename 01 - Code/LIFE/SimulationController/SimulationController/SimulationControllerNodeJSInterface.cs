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
        private readonly SimulationManagerClient _simulationManagerClient;

        public SimulationControllerNodeJsInterface() {
            _simulationManagerClient = new SimulationManagerClient();
        }

        public async Task<object> GetAllModels()
        {
            return await Task.Run(
                () => _simulationManagerClient.GetAllModels());
        }

        public async Task<object> StartSimulationWithModel(dynamic input)
        {
            return await Task.Run(
                () =>
                {
                    _simulationManagerClient.StartSimulationWithModel(input);
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
}
