using System.Collections.Generic;
using SMConnector.TransportTypes;

namespace SMConnector {
    public delegate void StatusUpdateAvailable(TStatusUpdate update);

    /// <summary>
    ///     Provides access to the SimulationManager's functions
    /// </summary>
    public interface ISimulationManager {
        /// <summary>
        ///     Returns an array of TModelDescriptions, describing all available models on the
        ///     SimulationManager node.
        /// </summary>
        /// <returns>A list of TModelDescriptions or an empty list if no models are present</returns>
        IList<TModelDescription> GetAllModels();

        /// <summary>
        ///     Starts a simulation with the model derived from the provided TModelDescription.
        /// </summary>
        /// <param name="model">The chosen model</param>
        void StartSimulationWithModel(TModelDescription model);

        /// <summary>
        ///     Subscribe for StatusUpdates from the SimulationManager.
        /// </summary>
        /// <param name="statusUpdateAvailable">
        ///     The callback method, which will be called
        ///     , when an update is available.
        /// </param>
        void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable);
    }
}