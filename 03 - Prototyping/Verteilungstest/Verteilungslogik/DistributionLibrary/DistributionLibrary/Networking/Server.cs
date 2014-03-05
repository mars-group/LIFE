using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DistributionLibrary.Settings;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class Server
    {
        private TcpListener serverSocket;

        private TcpClient clientSocket;

        private bool running;

        public Server()
        {
            string adress = Settings.NetworkSettings.HostingIP;
            int port = Settings.NetworkSettings.HostingPort;

            serverSocket = new TcpListener(IPAddress.Parse(adress), port);
            serverSocket.Start();

            running = true;
            new Thread(Greeter).Start();
        }

        private void Greeter()
        {
            while (running)
            {
                try
                {
                    clientSocket = serverSocket.AcceptTcpClient();

                    StreamReader reader = new StreamReader(clientSocket.GetStream());
                    //StreamWriter writer = new StreamWriter(clientSocket.GetStream());

                    Debug.LogWarning(reader.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        ~Server()
        {
            running = false;
        }
    }
}
