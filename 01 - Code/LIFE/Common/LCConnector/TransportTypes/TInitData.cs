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
    [Serializable]
    public class TInitData
    {
        public List<AgentInitConfig> AgentInitConfigs { get; private set; }

        public TInitData()
        {
            AgentInitConfigs = new List<AgentInitConfig>();
        }

        public void AddAgentInitConfig(string agentName, int agentAmount, int shadowAgentAmount, long lastIdOfRealAgent)
        {
            AgentInitConfigs.Add(new AgentInitConfig(agentName, agentAmount, shadowAgentAmount, lastIdOfRealAgent));
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

        /// <summary>
        /// The last Id of the real agents.
        /// Can be used to calculate the ranges and Ids of
        /// all shadow agents
        /// </summary>
        public long LastIdOfRealAgent { get; set; }

        public AgentInitConfig(string agentName, int agentCount, int shadowAgentCount, long lastIdOfRealAgent)
        {
            AgentName = agentName;
            RealAgentCount = agentCount;
            ShadowAgentCount = shadowAgentCount;
            LastIdOfRealAgent = lastIdOfRealAgent;
        }
    }
}