using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MulticastAdapter.Interface;

namespace MulticastAdapter.Implementation
{
    class UDPMulticastClient : IMulticastClientAdapter
    {
        private UdpClient client;
        private IPAddress mGrpAdr;
        private int port;


        public UDPMulticastClient(IPAddress ipAddress, int port)
        {
            this.mGrpAdr = ipAddress;
            this.port = port;
            this.client = new UdpClient(port, AddressFamily.InterNetwork);
            client.JoinMulticastGroup(mGrpAdr);
        }




        private void ValidateSocket()
        {
            throw new NotImplementedException();
        }

        public void CloseSocket()
        {
            client.DropMulticastGroup(mGrpAdr);
            client.Close();
        }

        public void ReopenSocket()
        {
            client = new UdpClient(port, AddressFamily.InterNetwork);
            client.JoinMulticastGroup(mGrpAdr);
        }

        public void ReopenSocket(IPAddress ipAddress, int _port)
        {
            this.mGrpAdr = ipAddress;
            this.port = _port;

            client = new UdpClient(port, AddressFamily.InterNetwork);
            client.JoinMulticastGroup(ipAddress);
        }


        public void SendMessageToMulticastGroup(byte[] msg)
        {
            Console.WriteLine("try to send Message under IP " + mGrpAdr + " Port " + port);
            client.Send(msg, msg.Length, new IPEndPoint(mGrpAdr, port));
            Console.WriteLine("Sending done");
        }

    }
}
