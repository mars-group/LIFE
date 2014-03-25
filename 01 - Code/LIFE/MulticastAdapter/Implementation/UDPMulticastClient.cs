using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MulticastAdapter.Interface;

namespace MulticastAdapter.Implementation
{
    public class UDPMulticastClient : IMulticastClientAdapter
    {
        private UdpClient client;
        private IPAddress mGrpAdr;
        private int port;



        public UDPMulticastClient()
        {
            this.mGrpAdr = IPAddress.Parse(ConfigurationManager.AppSettings.Get("IP"));
            this.port = Int32.Parse(ConfigurationManager.AppSettings.Get("Port"));
            this.client = new UdpClient(port, AddressFamily.InterNetwork);
            
          
            client.JoinMulticastGroup(mGrpAdr);

        }

        public UDPMulticastClient(IPAddress ipAddress, int port)
        {
            this.mGrpAdr = ipAddress;
            this.port = port;
            this.client = new UdpClient(port, AddressFamily.InterNetwork);
           
            client.JoinMulticastGroup(mGrpAdr);
        }


        private void BindSocketToInterface()
        {
            

            BindSocketToInterfaceByIP();
        }

        private void BindSocketToInterfaceByIP()
        {
            client.Client.Bind(new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings.Get("GetSendingInterfaceByIPv4")), port));
        }

        private void BindSocketToInterfaceByName()
        {
            throw new NotImplementedException();
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
            client.Send(msg, msg.Length, new IPEndPoint(mGrpAdr, port));
        }

    }
}
