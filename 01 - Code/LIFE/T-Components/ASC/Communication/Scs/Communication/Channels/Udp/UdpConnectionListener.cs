using ASC.Communication.Scs.Communication.EndPoints.Udp;

namespace ASC.Communication.Scs.Communication.Channels.Udp
{
    /// <summary>
    /// Will just raise the OnCommunicationChannelconnected Event to fullful the 
    /// SCS framework's requirements.
    /// </summary>
    class UdpConnectionListener : ConnectionListenerBase
    {
        private readonly ICommunicationChannel _udpchannel;

        public UdpConnectionListener(AscUdpEndPoint endpoint) {
            _udpchannel = new UdpCommunicationChannel(endpoint);
        }

        public override void Start() {
            OnCommunicationChannelConnected(_udpchannel);
        }

        public override void Stop() {
            // nothing to be done here
        }
    }
}
