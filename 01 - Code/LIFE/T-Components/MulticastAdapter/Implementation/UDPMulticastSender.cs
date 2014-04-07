using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using AppSettingsManager.Implementation;
using AppSettingsManager.Interface;
using AppSettingsManager.Interface.Exceptions;
using ConfigurationAdapter.Implementation;
using ConfigurationAdapter.Interface;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Config.Types;
using MulticastAdapter.Interface.Exceptions;

namespace MulticastAdapter.Implementation
{
    public class UDPMulticastSender : IMulticastClientAdapter
    {
        private IList<UdpClient> clients;
        private readonly IPAddress mGrpAdr;
        private readonly int sendingPort;
        private readonly int listenPort;
        private readonly Configuration<GeneralMulticastAdapterConfig> generalSettings;
        private readonly Configuration<MulticastSenderConfig> senderSettings;


        public UDPMulticastSender()
        {
            var path = "./" + typeof(UDPMulticastSender).Name;
            generalSettings = new Configuration<GeneralMulticastAdapterConfig>(path + "General.config");
            senderSettings = new Configuration<MulticastSenderConfig>(path + "sender.config");

            mGrpAdr = IPAddress.Parse(generalSettings.Content.MulticastGroupeIP);
            sendingPort = generalSettings.Content.SendingPort;
            listenPort = generalSettings.Content.ListenPort;
            clients = GetSendingInterfaces();
        }

        public UDPMulticastSender(IPAddress ipAddress, int sendingPort, int listenPort)
            : this()
        {
            var path = "./" + typeof(UDPMulticastSender).Name;
            generalSettings = new Configuration<GeneralMulticastAdapterConfig>(path);
            senderSettings = new Configuration<MulticastSenderConfig>(path);

            mGrpAdr = ipAddress;
            this.sendingPort = sendingPort;
            this.listenPort = listenPort;

            clients = GetSendingInterfaces();
        }

        private IList<UdpClient> GetSendingInterfaces()
        {
            IList<UdpClient> resultList = new List<UdpClient>();

            if (senderSettings.Content.SendOnAllInterfaces)
            {
                foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces())
                {
                    foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (unicastAddress.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily())
                        {
                            var updClient = new UdpClient(new IPEndPoint(unicastAddress.Address, sendingPort));
                            updClient.JoinMulticastGroup(mGrpAdr, unicastAddress.Address);
                            resultList.Add(updClient);
                        }
                    }
                }
            }
            else
            {
                var updClient = new UdpClient(GetBindingEndpoint());
                updClient.JoinMulticastGroup(mGrpAdr, IPAddress.Parse(senderSettings.Content.SendingInterfaceIP));
                resultList.Add(updClient);
            }


            return resultList;
        }


        private IPEndPoint GetBindingEndpoint()
        {
            switch (senderSettings.Content.BindingType)
            {
                case BindingType.IP:
                    return GetIPEndPointByIp();
                case BindingType.Name:
                    return GetIPEndPointByName();
                default:
                    // this shut never happend
                    throw new NotImplementedException("The type by which the binding interface is determined is not implemented.");
            }

        }

        private IPEndPoint GetIPEndPointByIp()
        {
            var networkInterface =
               MulticastNetworkUtils.GetInterfaceByIP(IPAddress.Parse(senderSettings.Content.SendingInterfaceIP));
            IPAddress ipAddress = null;

            foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (unicastAddress.Address.AddressFamily.Equals(MulticastNetworkUtils.GetAddressFamily()))
                {
                    ipAddress = unicastAddress.Address;
                    break;
                }
            }

            if (ipAddress != null) return new IPEndPoint(ipAddress, sendingPort);
            throw new NoInterfaceFoundException("No interface with the given IP " + ipAddress +
                                                " was found. Please check if your interface description, in app.config, is right.");
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
                if (client.Client != null) client.Send(msg, msg.Length, new IPEndPoint(mGrpAdr, listenPort));
            }
        }
    }
}