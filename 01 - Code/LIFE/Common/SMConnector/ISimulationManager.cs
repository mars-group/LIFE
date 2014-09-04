using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using CommonTypes.DataTypes;
using Hik.Communication.ScsServices.Service;
using SMConnector.TransportTypes;

namespace SMConnector {
    public delegate void StatusUpdateAvailable(TStatusUpdate update);

    /// <summary>
    ///     Provides access to the SimulationManager's functions
    /// </summary>
    [ScsService(Version = "0.1")]
    public interface ISimulationManager {
        
        /// <summary>
        ///     Returns a list of TModelDescriptions, describing all available models on the
        ///     SimulationManager node.
        /// </summary>
        /// <returns>A list of TModelDescriptions or an empty list if no models are present</returns>
        ICollection<TModelDescription> GetAllModels();

        /// <summary>
        /// Starts a simulation with the model derived from the provided TModelDescription.
        /// </summary>
        /// <param name="model">not null</param>
        /// <param name="layerContainers">The layer containers with which </param>
        /// <param name="nrOfTicks"></param>
        void StartSimulationWithModel(TModelDescription model, ICollection<TNodeInformation> layerContainers, int? nrOfTicks = null);

        /// <summary>
        ///     Holds the execution of the simulation with the given model indefinitely until it is either aborted or resumed.
        /// </summary>
        /// <param name="model">not null</param>
        void PauseSimulation(TModelDescription model);

        /// <summary>
        ///     Resumes the simulation with the given model, if there is one running. Does nothing, if there is not.
        /// </summary>
        /// <param name="model"></param>
        void ResumeSimulation(TModelDescription model);

        /// <summary>
        ///     Aborts a running simualtion with the given model, if one is running. Does nothing, if there is not.
        /// </summary>
        /// <param name="model"></param>
        void AbortSimulation(TModelDescription model);

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