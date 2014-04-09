using log4net;

namespace SimulationManagerController.Implementation {
    internal class SimulationInitializationUseCase {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (SimulationInitializationUseCase));

        public SimulationInitializationUseCase() {
            Logger.Debug("initialized.");
        }
    }
}