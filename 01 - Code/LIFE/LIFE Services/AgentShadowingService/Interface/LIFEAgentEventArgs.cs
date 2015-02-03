using System;
using System.Collections.Generic;

namespace AgentShadowingService.Interface
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
