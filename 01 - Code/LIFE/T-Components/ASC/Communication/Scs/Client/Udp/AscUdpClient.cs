﻿using ASC.Communication.Scs.Communication.Channels;
using ASC.Communication.Scs.Communication.Channels.Udp;
using ASC.Communication.Scs.Communication.EndPoints.Udp;

namespace ASC.Communication.Scs.Client.Udp
{
    internal class AscUdpClient : AscClientBase
    {
        private readonly AscUdpEndPoint _endPoint;
        private ICommunicationChannel _udpchannel;

        public AscUdpClient(AscUdpEndPoint endPoint)
        {
            _endPoint = endPoint;
        }

        protected override ICommunicationChannel CreateCommunicationChannel()
        {
            return _udpchannel ?? (_udpchannel = new UdpCommunicationChannel(_endPoint));
        }
    }
}
