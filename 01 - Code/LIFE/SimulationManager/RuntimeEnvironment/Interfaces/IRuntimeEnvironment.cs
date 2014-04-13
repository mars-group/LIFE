using System.Collections.Generic;
using CommonTypes.DataTypes;
using SMConnector;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Interfaces {
    /// <summary>
    /// TODO: comment
    /// </summary>
    public interface IRuntimeEnvironment {
        void StartSimulationWithModel(TModelDescription model, ICollection<NodeInformationType> layerContainers, int? nrOfTicks = null);
        void PauseSimulation(TModelDescription model);
        void ResumeSimulation(TModelDescription model);
        void AbortSimulation(TModelDescription model);
        void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable);
    }
}