//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using DalskiAgent.Agents;
using GeoGridEnvironment.Interface;
using LCConnector.TransportTypes;
using LifeAPI.Agent;
using LifeAPI.Layer;
using SpatialAPI.Environment;

namespace AgentManager.Interface
{
    public interface IAgentManager<T> where T : IAgent {
		IDictionary<Guid,T> GetAgentsByAgentInitConfig(
            AgentInitConfig agentInitConfig, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle, IEnvironment environment,
             List<ILayer> additionalLayerDependencies, IGeoGridEnvironment<GpsAgent> geoGridEnvironment = null,
            int reducedAgentCount = -1);
    }
}
