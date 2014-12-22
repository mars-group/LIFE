using ASC.Communication.Scs.Communication.Channels.Udp;
using Hik.Communication.Scs.Communication.Channels;
using Hik.Communication.Scs.Communication.Channels.Udp;
using Hik.Communication.Scs.Communication.EndPoints.Udp;

namespace Hik.Communication.Scs.Client.Udp
{
    internal class ScsUdpClient : ScsClientBase
    {
        private readonly ScsUdpEndPoint _endPoint;

        public ScsUdpClient(ScsUdpEndPoint endPoint)
        {
            _endPoint = endPoint;
        }

        protected override ICommunicationChannel CreateCommunicationChannel()
        {
            return new UdpCommunicationChannel(_endPoint);
        }
    }
}
