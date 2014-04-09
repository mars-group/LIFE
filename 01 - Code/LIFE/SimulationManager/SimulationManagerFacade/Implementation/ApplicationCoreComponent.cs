using ModelContainer.Interfaces;
using RuntimeEnvironment.Interfaces;
using SimulationManagerController.Interfaces;
using SimulationManagerFacade.Interface;

namespace SimulationManagerFacade.Implementation {
    public class ApplicationCoreComponent : IApplicationCore {
        private readonly ISimulationManagerController _simulationManagerController;
        private readonly IRuntimeEnvironment _runtimeEnvironment;
        private readonly IModelContainer _modelContainer;

        public ApplicationCoreComponent(ISimulationManagerController simulationManagerController,
                                        IRuntimeEnvironment runtimeEnvironment,
                                        IModelContainer modelContainer) {
            _simulationManagerController = simulationManagerController;
            _runtimeEnvironment = runtimeEnvironment;
            _modelContainer = modelContainer;
        }
    }
}