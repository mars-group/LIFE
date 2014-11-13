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
        /// Starts a simulation with the model derived from the provided TModelDescription.
        /// Will just initialize and then pause the execution if <param name="startPaused"/> is true.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="layerContainers"></param>
        /// <param name="startPaused"></param>
        /// <param name="nrOfTicks"></param>
        void StartSimulationWithModel(TModelDescription model, ICollection<TNodeInformation> layerContainers, bool startPaused, int? nrOfTicks = null);

		/// <summary>
		/// Steps the simulation by one tick or by nrOfTicks if provided.
		/// </summary>
		/// <param name="model">Model.</param>
		/// <param name="layerContainers">Layer containers.</param>
		/// <param name="nrOfTicks">Nr of ticks.</param>
        void StepSimulation(TModelDescription model, ICollection<TNodeInformation> layerContainers, int? nrOfTicks = null);

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
        /// Starts the visuzalization of the model.
        /// Optional: Provide an integer value describing
        /// the intervall of visualized ticks in case you do
        /// not want to visualize every tick.
        /// </summary>
        /// <param name="model">The model to visualize.</param>
        /// <param name="nrOfTicksToVisualize">The intervall in which to visualize.</param>
        void StartVisualization(TModelDescription model, int? nrOfTicksToVisualize = null);

        /// <summary>
        /// Stops the visualization for the given model.
        /// </summary>
        /// <param name="model"></param>
        void StopVisualization(TModelDescription model);

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