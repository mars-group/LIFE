using Nancy;
using SimulationManagerFacade.Interface;

namespace SimulationManagerWebservice
{
	public class ControlModule : NancyModule
	{
		public ControlModule (ISimulationManagerApplicationCore simManager)
		{
			Post ["/marscontrol/stop"] = _ => simManager.GetAllModels().Count;
		}
	}
}

