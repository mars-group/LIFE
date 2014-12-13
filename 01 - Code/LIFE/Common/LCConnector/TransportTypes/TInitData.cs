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

        public void AddAgentInitConfig(string agentName, int agentAmount)
        {
            AgentInitConfigs.Add(new AgentInitConfig(agentName, agentAmount));
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
        public int AgentCount { get; set; }

        public AgentInitConfig(string agentName, int agentCount)
        {
            AgentName = agentName;
            AgentCount = agentCount;
        }
    }
}