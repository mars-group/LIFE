using SMConnector;
using System.Collections.Generic;
using SMConnector.TransportTypes;
using CommonTypes.DataTypes;

namespace SimulationManagerFacade.Interface {
    /// <summary>
    /// TODO: comment
    /// </summary>
    public interface IApplicationCore :
                            ISimulationManager
    {
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
		/// <param name="nrOfTicks"></param>
		void StartSimulationWithModel(TModelDescription model, int? nrOfTicks = null);
    }
}