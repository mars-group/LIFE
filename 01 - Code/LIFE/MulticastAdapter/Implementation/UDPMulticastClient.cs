using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Exceptions;

namespace MulticastAdapter.Implementation
{
    public class UDPMulticastClient : IMulticastClientAdapter
    {
        private UdpClient client;
        private IPAddress mGrpAdr;
        private int sendingPort;
        private int listenPort;



        public UDPMulticastClient()
        {
            this.mGrpAdr = IPAddress.Parse(ConfigurationManager.AppSettings.Get("IP"));
            this.sendingPort = Int32.Parse(ConfigurationManager.AppSettings.Get("SendingPort"));
            this.listenPort = Int32.Parse(ConfigurationManager.AppSettings.Get("ListenPort"));
            this.client = new UdpClient(GetBindingEndpoint());

            GetBindingEndpoint();
            client.JoinMulticastGroup(mGrpAdr, IPAddress.Parse(ConfigurationManager.AppSettings.Get("GetSendingInterfaceByIPv4")));

        }

        public UDPMulticastClient(IPAddress ipAddress, int sendingPort, int listenPort)
        {
            this.mGrpAdr = ipAddress;
            this.sendingPort = sendingPort;
            this.listenPort = listenPort;
            this.client = new UdpClient(GetBindingEndpoint());

            GetBindingEndpoint();
            client.JoinMulticastGroup(mGrpAdr);
        }


        private IPEndPoint GetBindingEndpoint()
        {
            if ("ip".Equals(ConfigurationManager.AppSettings.Get("BindSendingInterfaceBy").ToLower()))
            {
                return GetIPEndPointByIp();
            }
            else if ("name".Equals(ConfigurationManager.AppSettings.Get("BindSendingInterfaceBy").ToLower()))
            {
                return GetIPEndPointByName();
            }

            throw new MissingArgumentException(" The Argument by which propertie the cientSocket should be bound to an interface is missinng. Use dd key=\"BindSendingInterfaceBy\" value=\"IP/Name\"/>");

        }

        private IPEndPoint GetIPEndPointByIp()
        {

            var networkInterface = MulticastNetworkUtils.GetInterfaceByIP(IPAddress.Parse(ConfigurationManager.AppSettings.Get("GetSendingInterfaceByIPv4")));
            IPAddress ipAddress = null;

            foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (unicastAddress.Address.AddressFamily.Equals(MulticastNetworkUtils.GetAddressFamily()))
                {
                    ipAddress = unicastAddress.Address;
                    break;
                }
            }

            if (ipAddress != null)
            {
              return new IPEndPoint(ipAddress, sendingPort);
            }
            else
            {
                throw new NoInterfaceFoundException("No interface with the given IP " + ipAddress + " was found. Please check if your interface description, in app.config, is right.");
            }


        }

        private IPEndPoint GetIPEndPointByName()
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
            client = new UdpClient(sendingPort, AddressFamily.InterNetwork);
            client.JoinMulticastGroup(mGrpAdr);
        }

        public void ReopenSocket(IPAddress ipAddress, int _port)
        {
            this.mGrpAdr = ipAddress;
            this.sendingPort = _port;

            client = new UdpClient(sendingPort, AddressFamily.InterNetwork);
            client.JoinMulticastGroup(ipAddress);
        }


        public void SendMessageToMulticastGroup(byte[] msg)
        {
            if (client.Client != null)
            {
                client.Send(msg, msg.Length, new IPEndPoint(mGrpAdr, listenPort));   
            }
           
        }

    }
}
