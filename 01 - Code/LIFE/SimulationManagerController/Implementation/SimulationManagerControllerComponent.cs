using SimulationManagerController.Interfaces;

namespace SimulationManagerController.Implementation
{
    

    public class SimulationManagerControllerComponent : ISimulationManagerController
    {
        private SimulationInitializationUseCase initializationUseCase;

        public SimulationManagerControllerComponent()
        {
            initializationUseCase = new SimulationInitializationUseCase();
        }
    }
}
