using System.Collections.Generic;
using CommonTypes.DataTypes;
using SMConnector;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Interfaces {
    /// <summary>
    /// TODO: comment
    /// </summary>
    public interface IRuntimeEnvironment {
        void StartWithModel(TModelDescription model, ICollection<TNodeInformation> layerContainers, int? nrOfTicks = null);
        void Pause(TModelDescription model);
        void Resume(TModelDescription model);
        void Abort(TModelDescription model);
        void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable);
    }
}