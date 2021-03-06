﻿using System;
using System.ComponentModel;
using System.Net;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using PhoneBookCommonLib;

namespace PhoneBookClient
{
    public class Client : IPhoneBookService
    {
        private readonly IScsServiceClient<IPhoneBookService> _client;
        private IYetAnotherService _yetAnotherClient;
        private const int BasePort = 10048;

        public Client(IYetAnotherService yetAnotherclient, Guid serviceID)
        {
            //Create a client to connect to phone book service on local server and
            //10048 TCP port.

            _client = ScsServiceClientBuilder.CreateClient<IPhoneBookService>(
                //new ScsTcpEndPoint(IPAddress.Loopback.ToString(), BasePort), serviceID);
                new ScsTcpEndPoint("192.168.178.31", BasePort));

            _yetAnotherClient = yetAnotherclient;

            //Connect to the server
            _client.Connect();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Title
        {
            get { return _client.ServiceProxy.Title; }
            set { _client.ServiceProxy.Title = value; }
        }

        public void AddPerson(PhoneBookRecord recordToAdd)
        {
            _client.ServiceProxy.AddPerson(recordToAdd);
        }

        public bool DeletePerson(string name)
        {
            return _client.ServiceProxy.DeletePerson(name);
        }

        public PhoneBookRecord FindPerson(string name)
        {
            return _client.ServiceProxy.FindPerson(name);
        }

        public string GetInformation()
        {
            return _yetAnotherClient.GetInformation();
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }
    }
}
