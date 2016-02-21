using System;
using System.Collections.Generic;
using LifeAPI.Agent;

namespace LifeAPI.Layer.Visualization
{
    public interface IVisualizableAgent : IAgent
    {
        /// <summary>
        /// Returns an appropiately formatted JSON string
        /// "{{"AgentID":"{0}","            // GUID of this agent.
        /// "AgentType":"{1}","             // Derived class type.
        /// "Position":[{2},{3},{4}],"      // Agent position (x,y,z).
        /// "Orientation":[{5},{6},{7}],"   // Rotation as (yaw,pitch,roll).
        /// "DisplayParams":{{{8}}}}}",     // Additional agent information.
        /// </summary>
        /// <returns></returns>
        string GetVisualizationJson();
    }
}
