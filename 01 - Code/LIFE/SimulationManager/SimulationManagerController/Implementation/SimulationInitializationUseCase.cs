using log4net;

namespace SimulationManagerController.Implementation {
    internal class SimulationInitializationUseCase {
        private static readonly ILog logger = LogManager.GetLogger(typeof (SimulationInitializationUseCase));

        public SimulationInitializationUseCase() {
            logger.Debug("initialized.");
        }
    }
}