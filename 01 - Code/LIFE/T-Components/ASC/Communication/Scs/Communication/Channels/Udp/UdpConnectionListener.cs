using ASC.Communication.Scs.Communication.EndPoints.Udp;

namespace ASC.Communication.Scs.Communication.Channels.Udp
{
    /// <summary>
    /// Will just raise the OnCommunicationChannelconnected Event to fullful the 
    /// SCS framework's requirements.
    /// </summary>
    class UdpConnectionListener : ConnectionListenerBase
    {
        private readonly AscUdpEndPoint _endPoint;
        private ICommunicationChannel _udpchannel;

        public UdpConnectionListener(AscUdpEndPoint endpoint) {
            _endPoint = endpoint;
        }

        public override void Start() {
            OnCommunicationChannelConnected(_udpchannel ?? (_udpchannel = new UdpCommunicationChannel(_endPoint)));
        }

        public override void Stop() {
            // nothing to be done here
        }
    }
}
