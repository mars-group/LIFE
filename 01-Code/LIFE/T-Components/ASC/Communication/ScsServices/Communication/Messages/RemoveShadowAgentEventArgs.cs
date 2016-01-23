namespace ASC.Communication.ScsServices.Communication.Messages
{
    public class RemoveShadowAgentEventArgs
    {
        public RemoveShadowAgentMessage RemoveShadowAgentMessage { get; private set; }

        public RemoveShadowAgentEventArgs(RemoveShadowAgentMessage message) {
            RemoveShadowAgentMessage = message;
        }
    }
}
