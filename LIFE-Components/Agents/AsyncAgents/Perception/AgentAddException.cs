using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace DalskiAgent.newAsycClasses
{
    [Serializable]
    class AgentAddException : Exception {
        private readonly Guid _agentId;
        public AgentAddException(string message, Guid agentId):
            base(message) {
            _agentId = agentId;
        }

        public Guid GetAgentId() {
            return _agentId;
        }
        protected AgentAddException(SerializationInfo info, StreamingContext context) 
            : base(info, context) {
            _agentId= (Guid)info.GetValue("_agentId",typeof(Guid));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("_agentId");
            info.AddValue("_agentId", _agentId);
            base.GetObjectData(info, context);
        }
    }
}
