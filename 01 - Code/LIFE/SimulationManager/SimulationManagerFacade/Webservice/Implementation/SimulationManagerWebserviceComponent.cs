using System;
using SMConnector;
using SimulationManagerFacade.Interface;

namespace SimulationManagerWebservice
{
	public class SimulationManagerWebserviceComponent : ISimulationManagerWebservice
	{

		private readonly ISimulationManagerWebservice _simulationManagerWebServiceUseCase;

		public SimulationManagerWebserviceComponent()
		{
			_simulationManagerWebServiceUseCase = new SimulationManagerWebserviceUseCase ();
		}

	}
}

