using System;
using ASC.Communication.Scs.Communication.Messages;

namespace ASC.Communication.ScsServices.Communication.Messages
{
    [Serializable]
    public class AddShadowAgentMessage : AscMessage
    {
        public Guid AgentID { get; set; }
    }
}
