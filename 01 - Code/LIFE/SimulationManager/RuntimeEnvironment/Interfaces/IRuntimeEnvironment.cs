using System.Collections.Generic;
using SMConnector;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Interfaces {
    /// <summary>
    /// TODO: comment
    /// </summary>
    public interface IRuntimeEnvironment {
        void StartSimulationWithModel(TModelDescription model, ICollection<int> layerContainers, int? nrOfTicks = null);
        void PauseSimulation(TModelDescription model);
        void ResumeSimulation(TModelDescription model);
        void AbortSimulation(TModelDescription model);
        void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable);
    }
}