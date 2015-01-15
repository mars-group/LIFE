using System;

namespace DMConnector.TransportTypes
{
    public class TShadowAgent
    {
        public TShadowAgent(Type agentType, Guid agentId) {
            AgentID = agentId;
            AgentType = agentType;
        }

        public Type AgentType { get; private set; }

        public Guid AgentID { get; private set; }

        
    }
}
