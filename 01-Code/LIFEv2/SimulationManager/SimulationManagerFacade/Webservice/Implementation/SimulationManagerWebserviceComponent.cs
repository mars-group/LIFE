//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 18.12.2015
//  *******************************************************/
using System;
using SMConnector;
using SimulationManagerFacade.Interface;
using SimulationManagerFacade.Webservice.Implementation;

namespace SimulationManagerWebservice
{
	public class SimulationManagerWebserviceComponent : ISimulationManagerWebservice
	{

		private readonly ISimulationManagerWebservice _simulationManagerWebServiceUseCase;

		public SimulationManagerWebserviceComponent(ISimulationManagerApplicationCore simManager)
		{
			_simulationManagerWebServiceUseCase = new SimulationManagerWebserviceUseCase (simManager);
		}

	}
}

