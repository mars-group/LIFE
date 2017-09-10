using System;
using System.Runtime.Serialization;

namespace AsyncAgents.Perception
{
    [Serializable]
    class AgentAddException : Exception {
        private readonly Guid _agentId;
        private SerializationInfo _info;
        private StreamingContext _context;
        public AgentAddException(string message, Guid agentId):
            base(message) {
            _agentId = agentId;
        }

        public Guid GetAgentId() {
            return _agentId;
        }
        protected AgentAddException(SerializationInfo info, StreamingContext context)
        {
            _info = info;
            _context = context;
            _agentId= (Guid)info.GetValue("_agentId",typeof(Guid));
        }


    }
}
