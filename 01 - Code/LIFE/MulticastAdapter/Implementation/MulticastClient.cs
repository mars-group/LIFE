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
    class MulticastClient : IMulticastClientAdapter
    {
        private UdpClient client;
        private IPAddress mGrpAdr;

        public MulticastClient(IPAddress ipAddress, int port)
        {
            this.mGrpAdr = ipAddress;
            this.client = new UdpClient(port, AddressFamily.InterNetworkV6);
            

        }

        public MulticastClient(string ipAddresse, int port)
        {
           throw new NotImplementedException();
        }

      

        public void JoinMulticastGroup(IPAddress ipAddressp)
        {

            //TODO fancy checks einbauen sobald es läuft
            IPv6MulticastOption multicastOption = new IPv6MulticastOption(ipAddressp);
           



            client.JoinMulticastGroup(mGrpAdr);
            
        }

        public void LeaveMulticastGroup(IPAddress ipAddressp)
        {
            client.DropMulticastGroup(ipAddressp);
        }

        public void SendMessageToMulticastGroup(byte[] msg)
        {
            client.Connect( );

            client.Send(msg, msg.Length);
        }


       
    }
}
