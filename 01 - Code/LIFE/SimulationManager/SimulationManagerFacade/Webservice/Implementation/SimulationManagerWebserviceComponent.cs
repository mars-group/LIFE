using System;
using SMConnector;
using SimulationManagerFacade.Interface;

namespace SimulationManagerWebservice
{
	public class SimulationManagerWebserviceComponent : ISimulationManagerWebservice
	{

		private readonly ISimulationManagerWebservice _simulationManagerWebServiceUseCase;

		public SimulationManagerWebserviceComponent(ISimulationManagerApplicationCore simManager)
		{
			_simulationManagerWebServiceUseCase = new SimulationManagerWebserviceUseCase (simManager);
		}

		#region ISimulationManagerWebservice implementation

		public void StartService ()
		{
			_simulationManagerWebServiceUseCase.StartService();
		}

		public void StopService ()
		{
			_simulationManagerWebServiceUseCase.StopService();
		}

		#endregion
	}
}

