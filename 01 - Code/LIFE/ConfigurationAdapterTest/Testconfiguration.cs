using System;
using ConfigurationAdapter.Interface;
using NUnit.Framework;
using Shared;

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
