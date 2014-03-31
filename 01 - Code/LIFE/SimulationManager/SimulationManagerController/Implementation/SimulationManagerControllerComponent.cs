using log4net;
using SimulationManagerController.Interfaces;

namespace SimulationManagerController.Implementation
{
    public class SimulationManagerControllerComponent : ISimulationManagerController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SimulationManagerControllerComponent));

        private SimulationInitializationUseCase initializationUseCase;

        public SimulationManagerControllerComponent()
        {
            initializationUseCase = new SimulationInitializationUseCase();
            logger.Debug("initialized.");
        }
    }
}
