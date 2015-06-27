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
using MARS.Shuttle.SimulationConfig;

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
        /// The amount of real-time which passes during one tick
        /// </summary>
        public TimeSpan OneTickTimeSpan { get; private set; }

        /// <summary>
        /// The wall clock date which marks the start of the simulation.
        /// </summary>
        public DateTime SimulationWallClockStartDate { get; private set; }

        /// <summary>
        /// The unique simulationId of this simulation run. Use this
        /// when using any of the LIFE services requiring a simrun id.
        /// </summary>
        public Guid SimulationId { get; private set; }

        /// <summary>
        /// Determines whethers this layer should be distributed or not.
        /// </summary>
        public bool Distribute { get; private set; }

        public GisInitConfig GisInitInfo { get; private set; }

        /// <summary>
        /// Creates a new TInitData object, with an empty list of AgentInitConfigs
        /// </summary>
        public TInitData(bool distribute, TimeSpan oneTickTimeSpan, DateTime simulationWallClockStartDate, Guid simulationId) {
            Distribute = distribute;
            OneTickTimeSpan = oneTickTimeSpan;
            SimulationWallClockStartDate = simulationWallClockStartDate;
            SimulationId = simulationId;
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
        /// <param name="agentInitParameters">Optional parameterinformation about how and from where
        ///     to initialize the agent's constructor parameters.</param>
        /// <param name="getMarsCubeUrl"></param>
        public void AddAgentInitConfig(string agentName, string agentFullName, int agentAmount, int shadowAgentAmount, Guid[] realAgentIds, Guid[] shadowAgentsIds, List<IAtConstructorParameter> agentInitParameters = null, string marsCubeUrl = null, string marsCubeName = null)
        {
            AgentInitConfigs.Add(new AgentInitConfig(agentName, agentFullName, agentAmount, shadowAgentAmount, realAgentIds, shadowAgentsIds, agentInitParameters, marsCubeUrl, marsCubeName));
        }

        public void AddGisInitConfig(string GisSourceUrl, string[] layerNames) {
            GisInitInfo = new GisInitConfig(GisSourceUrl, layerNames);
        }
    }

    [Serializable]
    public class GisInitConfig 
    {
        public string GisSourceUrl { get; set; }
        public string[] LayerNames { get; set; }

        public GisInitConfig(string gisSourceUrl, string[] layerNames) {
            GisSourceUrl = gisSourceUrl;
            LayerNames = layerNames;
        }
    }

    [Serializable]
    public class AgentInitConfig
    {
        /// <summary>
        /// The name of the agent
        /// </summary>
        public string AgentName { get; set; }

        public string AgentFullName { get; set; }

        /// <summary>
        /// The amount of agents to use in the simulation
        /// </summary>
        public int RealAgentCount { get; set; }

        /// <summary>
        /// The amount of shadow agents
        /// </summary>
        public int ShadowAgentCount { get; set; }

        /// <summary>
        /// The ids to be used for the real agents.
        /// </summary>
        public Guid[] RealAgentIds { get; set; }

        /// <summary>
        /// The ids to be used for the shadow agents
        /// </summary>
        public Guid[] ShadowAgentsIds { get; set; }

        /// <summary>
        /// The URL connection string for the MARS CUBE
        /// </summary>
        public string MarsCubeUrl { get; set; }

        /// <summary>
        /// The name of the specific cube in MARS ROCK to be used for initialization
        /// </summary>
        public string MarsCubeName { get; set; }

        /// <summary>
        /// Constructor call parameters and information about where to get the data from
        /// Check this for null, prior to using it!
        /// </summary>
        public List<IAtConstructorParameter> AgentInitParameters { get; set; }

        public AgentInitConfig(string agentName, string agentFullName, int agentCount, int shadowAgentCount, Guid[] realAgentIds, Guid[] shadowAgentsIds, List<IAtConstructorParameter> agentInitParameters = null, string marsCubeUrl = null, string marsCubeName = null)
        {
            AgentName = agentName;
            AgentFullName = agentFullName;
            RealAgentCount = agentCount;
            ShadowAgentCount = shadowAgentCount;
            ShadowAgentsIds = shadowAgentsIds;
            AgentInitParameters = agentInitParameters;
            RealAgentIds = realAgentIds;
            MarsCubeUrl = marsCubeUrl;
            MarsCubeName = marsCubeName;
        }


    }
}