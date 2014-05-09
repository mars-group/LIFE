using MulticastAdapter.Interface.Config;
using NodeRegistry.Interface;
using NUnit.Framework;
using RuntimeEnvironment.Implementation;
using Shared;
using SimulationManagerTest.MockComponents;

namespace SimulationManagerTest {
    [TestFixture]
    internal class RuntimeEnvironmentTest {
        [Test]
        public void TestSteppedSimulationExecution() {
            RuntimeEnvironmentComponent runtimeEnvironmentComponent = new RuntimeEnvironmentComponent
                (new SimulationManagerSettings("./", new NodeRegistryConfig(), new MulticastSenderConfig()),
                    new ModelContainerMock(),
                    new NodeRegistryMock());
        }
    }
}