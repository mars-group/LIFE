using ASC.Communication.Scs.Communication.Channels;

namespace ASC.Communication.Scs.Client.Udp
{
    internal class AscUdpClient : AscClientBase
    {
        private readonly ICommunicationChannel _udpchannel;

        public AscUdpClient(ICommunicationChannel udpChannel)
        {
            _udpchannel = udpChannel;
        }

        protected override ICommunicationChannel CreateCommunicationChannel() {
            return _udpchannel;
        }
    }
}
