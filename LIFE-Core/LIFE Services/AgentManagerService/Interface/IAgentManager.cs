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
using LIFE.API.Agent;
using LIFE.API.GeoCommon;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using LIFE.Components.Environments.GeoGridEnvironment;
using LIFE.Components.ESC.SpatialAPI.Environment;

namespace LIFE.Services.AgentManagerService.Interface
{
    public interface IAgentManager<T> where T : IAgent {
		IDictionary<Guid,T> GetAgentsByAgentInitConfig(
            AgentInitConfig agentInitConfig, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle, List<ILayer> additionalLayerDependencies,
            IEnvironment environment = null, IGeoGridEnvironment<IGeoCoordinate> geoGridEnvironment = null,
            int reducedAgentCount = -1);
    }
}
