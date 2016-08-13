//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
namespace LifeAPI.Config
{
    /// <summary>
    /// An agent configuration providing information about the agent's name and amount
    /// </summary>
    public class AgentConfig
    {
        /// <summary>
        /// The name of the agent
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// The amount of agents to use in the simulation
        /// </summary>
        public int AgentCount { get; set; }

        /// <summary>
        /// Basic constructor. Creates 0 "noname" agents.
        /// </summary>
        public AgentConfig() {
            AgentName = "noname";
            AgentCount = 0;
        }

        /// <summary>
        /// Create a new AgentConfig
        /// </summary>
        /// <param name="agentName">The agent's class name.</param>
        /// <param name="agentCount">The amount of agents to simulate.</param>
        public AgentConfig(string agentName, int agentCount)
        {
            AgentName = agentName;
            AgentCount = agentCount;
        }
    }
}
