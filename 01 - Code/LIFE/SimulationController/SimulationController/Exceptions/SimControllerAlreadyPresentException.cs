using System.Collections.Generic;
using System.Linq;
using AppSettingsManager;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using ConfigurationAdapter.Interface;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;
using NodeRegistry.Implementation;
using NodeRegistry.Interface;
using SMConnector;
using SMConnector.TransportTypes;

namespace SimulationController.Implementation
{
	public class SimControllerAlreadyPresentException : System.Exception
	{
		public SimControllerAlreadyPresentException(string ipAddress) : 
		base("There's already another SimController running on this network. Its IP Address is: " + ipAddress) {}
	}

}