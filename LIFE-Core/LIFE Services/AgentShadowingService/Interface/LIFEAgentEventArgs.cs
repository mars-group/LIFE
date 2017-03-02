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

namespace LIFE.Services.AgentShadowingService.Interface
{
    public class LIFEAgentEventArgs<TServiceInterface> : EventArgs
    {
        public LIFEAgentEventArgs(List<TServiceInterface> removedAgents, List<TServiceInterface> newAgents) {
            RemovedAgents = removedAgents;
            NewAgents = newAgents;
        }

        public List<TServiceInterface> NewAgents { get; private set; }
        public List<TServiceInterface> RemovedAgents { get; private set; } 
    }
}
