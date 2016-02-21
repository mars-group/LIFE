//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System.Collections.Generic;

namespace LifeAPI.Layer.Visualization {
    /// <summary>
    ///     Implement this interface if you want your Layer to be visualized after each tick
    /// </summary>
    public interface IVisualizableLayer : ILayer {

        /// <summary>
        /// Returns a List of approriately formatted JSON strings. The format is as defined as:
        /// "{{"AgentID":"{0}","            // GUID of this agent.
        /// "AgentType":"{1}","             // Derived class type.
        /// "Position":[{2},{3},{4}],"      // Agent position (x,y,z).
        /// "Orientation":[{5},{6},{7}],"   // Rotation as (yaw,pitch,roll).
        /// "DisplayParams":{{{8}}}}}",     // Additional agent information.
        /// </summary>
        /// <returns>A list of everything visualizable in the whole environment, empty list if nothing is present</returns>
        List<string> GetVisData();
    }
}