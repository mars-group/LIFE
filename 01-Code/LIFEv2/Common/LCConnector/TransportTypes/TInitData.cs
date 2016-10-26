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

namespace LCConnector.TransportTypes {
    /// <summary>
    /// Will be passed to each Layer in its InitLayer(...) method.
    /// Contains all relevant information to properly initialize this layer.
    /// </summary>
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

        /// <summary>
        /// A GIS initialize info object, containing all relevant informato to initialize
        /// a GIS layer. Might be NULL, so check!
        /// </summary>
        public GisInitConfig GisInitInfo { get; private set; }

        /// <summary>
        /// A TimeSeries initializing a TimeSeries Layer in LIFE.
        /// Might be NULL, so check!
        /// </summary>
        public TimeSeriesInitConfig TimeSeriesInitInfo { get; set; }

        public FileInitInfoConfig FileInitInfoConfig { get; set; }

        /// <summary>
        /// The address of the MARS Config. Defaults to http://marsconfig, but 
        /// will be overridden, in case the '--mca' flag is provided upon
        /// start.
        /// </summary>
        public string MARSConfigAddress { get; private set; }

        /// <summary>
        /// Creates a new TInitData object, with an empty list of AgentInitConfigs
        /// </summary>
        public TInitData(bool distribute, TimeSpan oneTickTimeSpan, DateTime simulationWallClockStartDate, Guid simulationId, string marsConfigAddress) {
            Distribute = distribute;
            OneTickTimeSpan = oneTickTimeSpan;
            SimulationWallClockStartDate = simulationWallClockStartDate;
            SimulationId = simulationId;
            MARSConfigAddress = marsConfigAddress;
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
        public void AddAgentInitConfig(string agentName, string agentFullName, int agentAmount, int offset, List<TConstructorParameterMapping> agentInitParameters = null)
        {
			AgentInitConfigs.Add(new AgentInitConfig(agentName, agentFullName, agentAmount, offset, agentInitParameters));
        }

		/// <summary>
		/// Adds a GisInitConfig to the configuration.
		/// </summary>
		/// <param name="GisSourceUrl">Gis source URL.</param>
		/// <param name="layerNames">Layer names.</param>
        public void AddGisInitConfig(string GisSourceUrl, string[] layerNames) {
            GisInitInfo = new GisInitConfig(GisSourceUrl, layerNames);
        }

		/// <summary>
		/// Adds a TimeSeriesInitConfig
		/// </summary>
		/// <param name="tableName">Table name.</param>
		/// <param name="databaseColumnName">Database column name.</param>
		/// <param name="clearColumnName">Clear column name.</param>
		/// <param name="timeSeriesStartTime">Time series start time.</param>
		public void AddTimeSeriesInitConfig(string tableName, string databaseColumnName, string clearColumnName,
            DateTime timeSeriesStartTime = default(DateTime)) {
			TimeSeriesInitInfo = new TimeSeriesInitConfig(tableName, databaseColumnName, clearColumnName, timeSeriesStartTime);
        }

        /// <summary>
        /// Adds a FileInitInfo for Layers which are initialized from a file downloaded from the MARS Cloud
        /// </summary>
        /// <param name="dataId">The uuidv4 identifing the file</param>
        public void AddFileInitInfo(string dataId)
        {
            FileInitInfoConfig = new FileInitInfoConfig(dataId);
        }
    }

    public class FileInitInfoConfig
    {
        public string FileId { get; private set; }

        public FileInitInfoConfig(string fileId)
        {
            FileId = fileId;
        }
    }

	/// <summary>
	/// Time series init config.
	/// </summary>
    public class TimeSeriesInitConfig {
		/// <summary>
		/// The name of the table inside the timeseries database
		/// </summary>
		/// <value>The name of the table.</value>
        public string TableName { get; set; }

		/// <summary>
		/// The name of the column as present in the Database.
		/// Use this to fetch data from the DB. 
		/// </summary>
		/// <value>The name of the database column.</value>
        public string DatabaseColumnName { get; set; }

		/// <summary>
		/// The clear name of column.
		/// Use this to present information to the user.
		/// </summary>
		/// <value>The name of the clear column.</value>
		public string ClearColumnName { get; set;}

        /// <summary>
        /// Might not be set.
		/// Check for default(DateTime)!
        /// </summary>
        private DateTime TimeSeriesStartTime { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LCConnector.TransportTypes.TimeSeriesInitConfig"/> class.
		/// </summary>
		/// <param name="tableName">Table name in timeseries db.</param>
		/// <param name="databaseColumnName">Database column name.</param>
		/// <param name="clearColumnName">Clear column name. For enduser presentation only!</param>
		/// <param name="timeSeriesStartTime">(Optional) Time series start time.</param>
		public TimeSeriesInitConfig(string tableName, string databaseColumnName, string clearColumnName, DateTime timeSeriesStartTime = default(DateTime)) {
            TableName = tableName;
            DatabaseColumnName = databaseColumnName;
			ClearColumnName = clearColumnName;
			TimeSeriesStartTime = timeSeriesStartTime;
        }
    }

	/// <summary>
	/// Gis init config.
	/// </summary>
    public class GisInitConfig 
    {
		/// <summary>
		/// The GIS source url used to retreive the gis file
		/// </summary>
		/// <value>The gis source URL.</value>
        public string GisSourceUrl { get; set; }
        
		/// <summary>
		/// The names of all layers the gis source is valid for.
		/// </summary>
		/// <value>The layer names.</value>
		public string[] LayerNames { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LCConnector.TransportTypes.GisInitConfig"/> class.
		/// </summary>
		/// <param name="gisSourceUrl">Gis source URL.</param>
		/// <param name="layerNames">Layer names.</param>
        public GisInitConfig(string gisSourceUrl, string[] layerNames) {
            GisSourceUrl = gisSourceUrl;
            LayerNames = layerNames;
        }
    }

	/// <summary>
	/// Agent init config.
	/// Used in the AgentManager to instantiate agents.
	/// </summary>
    public class AgentInitConfig
    {
        /// <summary>
        /// The name of the agent
        /// </summary>
        public string AgentName { get; set; }

		/// <summary>
		/// The full name of the agent.
		/// </summary>
		/// <value>The name of the agent full.</value>
        public string AgentFullName { get; set; }

        /// <summary>
        /// The amount of agents to use in the simulation
        /// </summary>
        public int RealAgentCount { get; set; }

		/// <summary>
		/// Offset to define where to start the fetching
		/// process during automatic initialization of agents.
		/// Defaults to 0.
		/// </summary>
		/// <value>The agent init offset.</value>
		public int AgentInitOffset { get; set; }

        /// <summary>
        /// Constructor call parameters and information about where to get the data from
        /// Check this for null, prior to using it!
        /// </summary>
        public List<TConstructorParameterMapping> AgentInitParameters { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LCConnector.TransportTypes.AgentInitConfig"/> class.
		/// </summary>
		/// <param name="agentName">Agent name.</param>
		/// <param name="agentFullName">Agent full name.</param>
		/// <param name="agentCount">Agent count.</param>
		/// <param name="shadowAgentCount">Shadow agent count.</param>
		/// <param name="realAgentIds">Real agent identifiers.</param>
		/// <param name="shadowAgentsIds">Shadow agents identifiers.</param>
		/// <param name="agentInitParameters">Agent init parameters.</param>
        public AgentInitConfig(string agentName, string agentFullName, int agentCount, int offset=0, List<TConstructorParameterMapping> agentInitParameters = null)
        {
            AgentName = agentName;
            AgentFullName = agentFullName;
            RealAgentCount = agentCount;
            AgentInitParameters = agentInitParameters;
			AgentInitOffset = offset;
        }


    }
}