using System.Collections.Generic;
using CommonTypes.DataTypes;
using CommonTypes.TransportTypes;
using SMConnector;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Interfaces {
    /// <summary>
    /// TODO: comment
    /// </summary>
    public interface IRuntimeEnvironment {
        void StartWithModel(TModelDescription model, ICollection<TNodeInformation> layerContainerNodes, int? nrOfTicks = null);

        void StepSimulation(TModelDescription model, ICollection<TNodeInformation> layerContainerNodes, int? nrOfTicks = null);
        void Pause(TModelDescription model);
        void Resume(TModelDescription model);
        void Abort(TModelDescription model);

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

        void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable);
    }
}