using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using PhoneBookCommonLib;

namespace PhoneBookClient
{
    public class Client : IPhoneBookService
    {
        private IScsServiceClient<IPhoneBookService> _client;
        private int _basePort = 10048;

        public Client()
        {
            //Create a client to connect to phone book service on local server and
            //10048 TCP port.

            _client = ScsServiceClientBuilder.CreateClient<IPhoneBookService>(
                new ScsTcpEndPoint(IPAddress.IPv6Loopback.ToString(), _basePort)
                );

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

        public void Disconnect()
        {
            _client.Disconnect();
        }
    }
}
