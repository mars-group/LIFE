using System;

namespace SimulationController
{
	public class NoSimulationManagerConnectedException : Exception
	{
		public NoSimulationManagerConnectedException() : base("There is no SimulationManager connected. Please start one up and/or check your network configuration.")
		{

		}
	}
}

