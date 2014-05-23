using System;
using MulticastAdapter.Interface.Config;
using NodeRegistry.Interface;

namespace SimulationManagerShared
{
    using CommonTypes.Types;

    /// <summary>
    /// This class holds all local settings for the SimulationManager.
    /// </summary>
    [Serializable]
    public class SimulationManagerSettings
    {
        public string ModelDirectoryPath { get; set; }
        public NodeRegistryConfig NodeRegistryConfig { get; set; }
        public MulticastSenderConfig MulticastSenderConfig { get; set; }

        public SimulationManagerSettings(string modelDirectoryPath, NodeRegistryConfig nodeRegistryConfig, MulticastSenderConfig multicastSenderConfig) {
            ModelDirectoryPath = modelDirectoryPath;
            NodeRegistryConfig = nodeRegistryConfig;
            MulticastSenderConfig = multicastSenderConfig;
        }

        //TODO: can this be internal?
        public SimulationManagerSettings() {
            ModelDirectoryPath = "./Models";
            NodeRegistryConfig = new NodeRegistryConfig(NodeType.SimulationManager, "SM-1","127.0.0.1",44521, true);
            MulticastSenderConfig = new MulticastSenderConfig();
        }
    }
}
