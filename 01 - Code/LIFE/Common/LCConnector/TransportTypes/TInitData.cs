// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

using System;
using System.Collections.Generic;

namespace LCConnector.TransportTypes {
    /// <summary>
    /// Will be passed to each Layer in its InitLayer(...) method.
    /// Contains all relevant information to properly initialize this layer.
    /// </summary>
    [Serializable]
    public class TInitData
    {
        /// <summary>
        /// A list of configurations for each agent type in the layer
        /// </summary>
        public List<AgentInitConfig> AgentInitConfigs { get; private set; }

        /// <summary>
        /// The 
        /// </summary>
        public TimeSpan OneTickTimeSpan { get; private set; }

        /// <summary>
        /// Creates a new TInitData object, with an empty list of AgentInitConfigs
        /// </summary>
        public TInitData()
        {
            AgentInitConfigs = new List<AgentInitConfig>();
        }

        /// <summary>
        /// Adds a new AgentInitConfig by providing all necessary information
        /// </summary>
        /// <param name="agentName">The name of the agent's class (not interface)</param>
        /// <param name="agentAmount">The amount of rel agents to be initialized</param>
        /// <param name="shadowAgentAmount">The amount of shadow agents to be initialized</param>
        /// <param name="realAgentIds">A Guid[] with ids to be used by the real agents</param>
        /// <param name="shadowAgentsIds">A Guid[] with ids to be used by the shadow agents</param>
        public void AddAgentInitConfig(string agentName, int agentAmount, int shadowAgentAmount, Guid[] realAgentIds, Guid[] shadowAgentsIds)
        {
            AgentInitConfigs.Add(new AgentInitConfig(agentName, agentAmount, shadowAgentAmount, realAgentIds, shadowAgentsIds));
        }
    }

    [Serializable]
    public class AgentInitConfig
    {
        /// <summary>
        /// The name of the agent
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// The amount of agents to use in the simulation
        /// </summary>
        public int RealAgentCount { get; set; }

        /// <summary>
        /// The amount of shadow agents
        /// </summary>
        public int ShadowAgentCount { get; set; }

        public Guid[] RealAgentIds { get; set; }

        public Guid[] ShadowAgentsIds { get; set; }

        public AgentInitConfig(string agentName, int agentCount, int shadowAgentCount, Guid[] realAgentIds, Guid[] shadowAgentsIds)
        {
            AgentName = agentName;
            RealAgentCount = agentCount;
            ShadowAgentCount = shadowAgentCount;
            ShadowAgentsIds = shadowAgentsIds;
            RealAgentIds = realAgentIds;
        }
    }
}