﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using AppSettingsManager.Implementation;
using AppSettingsManager.Interface;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Exceptions;

namespace MulticastAdapter.Implementation
{
    public class UDPMulticastSender : IMulticastClientAdapter
    {
        private IList<UdpClient> clients;
        private IPAddress mGrpAdr;
        private int sendingPort;
        private int listenPort;
        private IConfigurationAdapter configuration;


        public UDPMulticastSender()
        {

            this.configuration = new NiniAdapterImpl("MulticastAdapter");
            this.mGrpAdr = configuration.GetIpAddress("IP");
            this.sendingPort = configuration.GetInt32("SendingPort");
            this.listenPort = configuration.GetInt32("ListenPort");
            this.clients = GetSendingInterfaces();

        }

        public UDPMulticastSender(IPAddress ipAddress, int sendingPort, int listenPort)
        {
            this.configuration = new NiniAdapterImpl("MulticastAdapter");
            this.mGrpAdr = ipAddress;
            this.sendingPort = sendingPort;
            this.listenPort = listenPort;
            this.clients = GetSendingInterfaces();

        }

        private IList<UdpClient> GetSendingInterfaces()
        {

            IList<UdpClient> resultList = new List<UdpClient>();

            if (configuration.GetBoolean("SendOnAllInterfaces"))
            {
                foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces())
                {
                    foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (unicastAddress.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily())
                        {
                            var updClient = new UdpClient(new IPEndPoint(unicastAddress.Address, sendingPort));
                            updClient.JoinMulticastGroup(mGrpAdr, configuration.GetIpAddress("GetSendingInterfaceByIPv4"));
                            resultList.Add(updClient);
                        }
                    }
                }
            }
            else
            {
                var updClient = new UdpClient(GetBindingEndpoint());
                updClient.JoinMulticastGroup(mGrpAdr, configuration.GetIpAddress("GetSendingInterfaceByIPv4"));
                resultList.Add(updClient);
            }


            return resultList;
        }


        private IPEndPoint GetBindingEndpoint()
        {
            if ("ip".Equals(configuration.GetValue("BindSendingInterfaceBy").ToLower()))
            {
                return GetIPEndPointByIp();
            }
            else if ("name".Equals(configuration.GetValue("BindSendingInterfaceBy").ToLower()))
            {
                return GetIPEndPointByName();
            }

            throw new MissingArgumentException(" The Argument by which propertie the cientSocket should be bound to an interface is missinng. Use dd key=\"BindSendingInterfaceBy\" value=\"IP/Name\"/>");

        }

        private IPEndPoint GetIPEndPointByIp()
        {

            var networkInterface = MulticastNetworkUtils.GetInterfaceByIP(configuration.GetIpAddress("GetSendingInterfaceByIPv4"));
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
            foreach (var client in clients)
            {
                client.DropMulticastGroup(mGrpAdr);
                client.Close();
            }


        }

        public void ReopenSocket()
        {
            CloseSocket();
            clients = GetSendingInterfaces();
        }




        public void SendMessageToMulticastGroup(byte[] msg)
        {
            foreach (var client in clients)
            {
                if (client.Client != null)
                {
                    client.Send(msg, msg.Length, new IPEndPoint(mGrpAdr, listenPort));
                }
            }

        }

    }
}
