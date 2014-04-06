namespace SimulationController
{
    using SMConnector.TransportTypes;
    using System.Threading.Tasks;

    public class SimulationControllerNodeJsInterface
    {
        private readonly SimulationManagerClient _simulationManagerClient;

        public SimulationControllerNodeJsInterface() {
            _simulationManagerClient = new SimulationManagerClient();
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
