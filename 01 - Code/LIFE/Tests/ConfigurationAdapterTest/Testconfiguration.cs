using ConfigurationAdapter.Interface;
using NUnit.Framework;
using SimulationManagerShared;

namespace ConfigurationAdapterTest
{
    public class TestConfiguration
    {
        [Test]
        public void TestConfigurationCreation() {
            Configuration.Load<SimulationManagerSettings>();
        }
    }
}
