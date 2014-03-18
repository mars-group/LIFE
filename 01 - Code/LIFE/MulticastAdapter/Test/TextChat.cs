using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            IPAddress multicastAddress = IPAddress.Parse("ff02::1");
            ASCIIEncoding ASCII = new ASCIIEncoding();

            IMulticastClientAdapter multicastClient = new Implementation.MulticastClient(multicastAddress, port);
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

        private MulticastReciever multicastReciever;
       
        public TextReciever()
        {
            multicastReciever = new MulticastReciever(IPAddress.Parse("ff02::1"), 50101);
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
