using System.IO;
using System.Net.Sockets;
using Assets.Scripts.Networking.Messages;
using DistributionLibrary.Settings;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    class Client
    {
        private TcpClient clientSocket;
        private StreamWriter writer;

        public Client()
        {
            clientSocket = new TcpClient(Settings.PeerNodeSettings[0].Adress, Settings.PeerNodeSettings[0].Port);

            writer = new StreamWriter(clientSocket.GetStream());
            writer.AutoFlush = true;
            writer.WriteLine("Hello, world :)");

            Debug.Log("Message versandt");
        }

        public void SendPeerConnectedMessage(PeerNodeConnectedMessage message)
        {

        }

    }
}
