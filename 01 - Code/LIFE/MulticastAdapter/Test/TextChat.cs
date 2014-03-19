using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface;


namespace MulticastAdapter.Test
{
    class TextChat
    {


        public static void Main(string[] args)
        {

            int port = 50100;
            string input = "";
            IPAddress multicastAddress = IPAddress.Parse("224.0.0.1");
            ASCIIEncoding ASCII = new ASCIIEncoding();
          
            //Console.WriteLine("IP is " + multicastAddress + " from type " + multicastAddress.GetType() + " this adress is linklocal " + multicastAddress.IsIPv6LinkLocal);
            //Console.WriteLine("this address is a IPv6 Multicastaddress: " + multicastAddress.IsIPv6Multicast);
            
            IMulticastClientAdapter multicastClient = new Implementation.UDPMulticastClient(multicastAddress, port);
            TextReciever textReciever = new TextReciever();
            
            Console.WriteLine("<<Hello, willkommen zum super aufregenden Multicastchat");
            Console.Write(">>");
            input = Console.ReadLine();
            
            multicastClient.SendMessageToMulticastGroup(ASCII.GetBytes(input));
            
            Thread listenThread= new Thread(new ThreadStart(textReciever.readNextMessage));
            
            listenThread.Start();
            
        }
        
    }

    class TextReciever
    {

        private UDPMulticastReciever multicastReciever;
       
        public TextReciever()
        {
            multicastReciever = new UDPMulticastReciever(IPAddress.Any, 50101);
        }

        public void readNextMessage()
        {

            ASCIIEncoding ascii = new ASCIIEncoding();

            byte[] msg;
            msg = multicastReciever.readMulticastGroupMessage();
            Console.WriteLine((ascii.GetString(msg)));

        }
        
    }

}
