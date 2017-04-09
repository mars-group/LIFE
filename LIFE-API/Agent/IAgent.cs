//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;

namespace LIFE.API.Agent
{
    /// <summary>
    ///   The basic IAgent interface.
    /// </summary>
    public interface IAgent : ITickClient
    {
        /// <summary>
        ///   The agent's main ID. Will be unique across the whole simulation.
        ///   Use this to reference the agent by ID.
        /// </summary>
        Guid ID { get; }
    }
}