//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;

namespace LIFE.API.Config
{
    /// <summary>
    ///   Defines the types of distribution strategies usable in a MARS LIFE simulation.
    /// </summary>
    [Serializable]
    public enum DistributionStrategy
    {
        /// <summary>
        ///   No distribution is applied.
        /// </summary>
        NO_DISTRIBUTION,

        /// <summary>
        ///   All entities will be evenly distributed across all nodes.
        /// </summary>
        EVEN_DISTRIBUTION,

        /// <summary>
        ///   Only valid on environmental layers using the ESC!
        ///   Will activate replication of environment
        /// </summary>
        ENV_REPLICATION
    }
}