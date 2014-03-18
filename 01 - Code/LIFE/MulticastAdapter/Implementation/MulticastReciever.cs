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
    class MulticastReciever : IMulticastReciever
    {

        private UdpClient recieverClient;
        private IPEndPoint senderEndPoint;


        public MulticastReciever(IPAddress mCastAdr, int sourcePort)
        {
            recieverClient = new UdpClient(AddressFamily.InterNetworkV6);
            recieverClient.JoinMulticastGroup(mCastAdr);
            senderEndPoint = new IPEndPoint(IPAddress.IPv6Any, sourcePort);

        }
        

        public byte[] readMulticastGroupMessage()
        {
            
            return recieverClient.Receive(ref senderEndPoint);
        }
    }
}
