using System.Collections.Generic;
using LNSConnector.Interface;
using SMConnector;
using SMConnector.TransportTypes;

namespace SimulationManagerFacade.Interface {
    /// <summary>
    /// The SimulationManager main core.
    /// </summary>
    public interface ISimulationManagerApplicationCore :
                            ISimulationManager, ILayerNameService
    {
		/// <summary>
		///     Returns a list of TModelDescriptions, describing all available models on the
		///     SimulationManager node.
		/// </summary>
		/// <returns>A list of TModelDescriptions or an empty list if no models are present</returns>
		ICollection<TModelDescription> GetAllModels();




        /// <summary>
        /// Steps a simulation with the model derived from the provided TModelDescription 
        /// If the simulation has not been started yet, it will be initialized and started.
        /// the default step width will be a single tick. If nrOfTicks is set, this number of ticks
        /// will be stepped.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="nrOfTicks"></param>
        void StepSimulation(TModelDescription model, int? nrOfTicks = null);
    }
}