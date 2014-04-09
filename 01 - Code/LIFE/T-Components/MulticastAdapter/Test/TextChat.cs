using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface;

namespace MulticastAdapter.Test {
    internal class TextChat {
        public static void Main(string[] args) {
            string input = "";

            ASCIIEncoding ASCII = new ASCIIEncoding();
            TextReciever textReciever = new TextReciever();


            Thread listenThread = new Thread(textReciever.readNextMessage);

            listenThread.Start();

            Thread.Sleep(200);

            //Console.WriteLine("IP is " + multicastAddress + " from type " + multicastAddress.GetType() + " this adress is linklocal " + multicastAddress.IsIPv6LinkLocal);
            //Console.WriteLine("this address is a IPv6 Multicastaddress: " + multicastAddress.IsIPv6LinkLocal);


            List<NetworkInterface> multicastInterfaces = new List<NetworkInterface>();

            IMulticastSender multicastClient = new UDPMulticastSender();
            Console.WriteLine("<<Hello, willkommen zum super aufregenden Multicastchat");
            Console.WriteLine("<<Type 'quit' to Exit the chat. ");

            while (input != "quit") {
                Console.Write(">>");
                input = Console.ReadLine();
                if (input != "quit") multicastClient.SendMessageToMulticastGroup(ASCII.GetBytes(input));
            }

            listenThread.Interrupt();
        }
    }

    internal class TextReciever {
        private readonly UDPMulticastReceiver multicastReciever;

        public TextReciever() {
            multicastReciever = new UDPMulticastReceiver();
        }

        public void readNextMessage() {
            ASCIIEncoding ascii = new ASCIIEncoding();

            byte[] msg;
            Console.WriteLine("waiting for Message");

            while (Thread.CurrentThread.IsAlive) {
                msg = multicastReciever.readMulticastGroupMessage();
                Console.WriteLine(("<<" + ascii.GetString(msg)));
            }

            multicastReciever.CloseSocket();
        }
    }
}