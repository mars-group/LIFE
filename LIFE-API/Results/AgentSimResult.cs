using System.Collections.Generic;

namespace LIFE.API.Results
{
    /// <summary>
    ///   Result structure for agent properties.
    /// </summary>
    public struct AgentSimResult
    {
        /// <summary>
        ///   GUID of this agent.
        /// </summary>
        public string AgentId;

        /// <summary>
        ///   Derived class type.
        /// </summary>
        public string AgentType;

        /// <summary>
        ///   The layer this agent lives on.
        /// </summary>
        public string Layer;

        /// <summary>
        ///   Simulation execution tick.
        /// </summary>
        public long Tick;

        /// <summary>
        ///   Agent position (lat, lon).
        /// </summary>
        public object Position;

        /// <summary>
        ///   Rotation as (yaw,pitch,roll).
        /// </summary>
        public double[] Orientation;

        /// <summary>
        ///   Additional agent information. 
        /// </summary>
        public IDictionary<string, object> AgentData;
    }
}