using System;

namespace ASC.Communication.ScsServices.Communication.Messages
{
    public class AddShadowAgentEventArgs : EventArgs
    {
        public AddShadowAgentMessage AddShadowAgentMessage { get; private set; }

        public AddShadowAgentEventArgs(AddShadowAgentMessage message) {
            AddShadowAgentMessage = message;
        }
    }
}
