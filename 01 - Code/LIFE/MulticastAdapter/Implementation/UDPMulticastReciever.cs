using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MulticastAdapter.Interface;

namespace MulticastAdapter.Implementation
{
    class UDPMulticastReciever : IMulticastReciever
    {

        private UdpClient recieverClient;
        private IPEndPoint senderEndPoint;


        public UDPMulticastReciever(IPAddress mCastAdr, int sourcePort)
        {
            recieverClient = new UdpClient(AddressFamily.InterNetwork);
            recieverClient.JoinMulticastGroup(mCastAdr);
            senderEndPoint = new IPEndPoint(IPAddress.Any, sourcePort);
            recieverClient.Client.Bind(new IPEndPoint(IPAddress. Parse("10.0.0.6"), sourcePort));
        }


        public byte[] readMulticastGroupMessage()
        {

            return recieverClient.Receive(ref senderEndPoint);

        }
    }
}
