using System;
using ASC.Communication.Scs.Communication.Messages;

namespace ASC.Communication.ScsServices.Communication.Messages
{
    [Serializable]
    public class RemoveShadowAgentMessage : AscMessage
    {
        public Guid AgentID { get; set; }
    }
}
