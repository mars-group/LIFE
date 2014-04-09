using System;

namespace Shared
{
    /// <summary>
    /// This class holds all local settings for the SimulationManager.
    /// </summary>
    [Serializable]
    public class SimulationManagerSettings
    {
        public string ModelDirectoryPath { get; set; }

        public SimulationManagerSettings(string modelDirectoryPath) {
            ModelDirectoryPath = modelDirectoryPath;
        }

        public SimulationManagerSettings() {}
    }
}
