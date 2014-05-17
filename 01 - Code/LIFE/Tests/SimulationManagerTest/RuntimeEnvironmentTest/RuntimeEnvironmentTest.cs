using System.Collections.Generic;
using System.Linq;
using ModelContainer.Interfaces;
using NUnit.Framework;
using RuntimeEnvironment.Implementation;
using RuntimeEnvironment.Implementation.Entities;
using SimulationManagerTestClasses.MockComponents;
using SMConnector.TransportTypes;

namespace SimulationManagerTestClasses.RuntimeEnvironmentTest {
    [TestFixture]
    internal class RuntimeEnvironmentTest {
        [Test]
        public void TestSteppedSimulationExecution() {
            IModelContainer modelContainer = new ModelContainerMock();
            TModelDescription model = modelContainer.GetAllModels().FirstOrDefault();
            IList<LayerContainerClient> clients = new LayerContainerClient[] { };
            SteppedSimulationExecutionUseCase steppedSimulation = new SteppedSimulationExecutionUseCase(50, clients);
        }
    }
}