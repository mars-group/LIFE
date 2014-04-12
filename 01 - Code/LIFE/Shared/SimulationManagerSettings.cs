using System;
using MulticastAdapter.Interface.Config;
using NodeRegistry.Interface;

namespace Shared
{
    /// <summary>
    /// This class holds all local settings for the SimulationManager.
    /// </summary>
    [Serializable]
    public class SimulationManagerSettings
    {
        public string ModelDirectoryPath { get; set; }
        public NodeRegistryConfig NodeRegistryConfig { get; set; }
        public MulticastSenderConfig MulticastSenderConfig { get; set; }

        public SimulationManagerSettings(string modelDirectoryPath, NodeRegistryConfig nodeRegistryConfig) {
            ModelDirectoryPath = modelDirectoryPath;
            NodeRegistryConfig = nodeRegistryConfig;
        }

        //TODO: can this be internal?
        public SimulationManagerSettings() {
            ModelDirectoryPath = "./Models";
            NodeRegistryConfig = new NodeRegistryConfig();
            MulticastSenderConfig = new MulticastSenderConfig();
        }
    }
}
