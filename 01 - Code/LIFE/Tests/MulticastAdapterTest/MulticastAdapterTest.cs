using System;
using System.CodeDom;
using System.Net;
using System.Threading;
using ConfigurationAdapter.Interface;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Config.Types;
using NUnit.Framework;


namespace MulticastAdapterTestProject
{
    using AppSettingsManager;

    public class MulticastAdapterTest
    {


        [Test]
        public void TestPortIncrement()
        {
            var startListenPort = 50555;

            var globalConfig = new GlobalConfig("224.10.100.1", 60543, startListenPort, 4);
            var senderConfig = new MulticastSenderConfig();

            //test if both adapter  can handle the same starting port
            var adapter1 = new MulticastAdapterComponent(globalConfig, senderConfig);
            var adapter2 = new MulticastAdapterComponent(globalConfig, senderConfig);

            // if no exceptions are thrown we are good to go
            Assert.True(true);

        }
        /*
        [Test]
        public void SendExcaltyOneMessageTest()
        {

            var testListenPort = 60030;
            var testSendIngPortStartSeed = 60000;
            var mcastAddress = "224.50.50.50";

            var reciever = new UDPMulticastReceiver(IPAddress.Parse(mcastAddress), testListenPort);
            var sender =
                new UDPMulticastSender(new GlobalConfig(mcastAddress, testListenPort, testSendIngPortStartSeed, 4),
                    new MulticastSenderConfig(false, "Ethernet 2", "",  BindingType.Name));

            var messageCounter  = new MessageCounter(reciever);

            var listenThread = new Thread(messageCounter.ListenAndCount);
            listenThread.Start();

            sender.SendMessageToMulticastGroup(new byte[0]);

            //wait for message to arraive.
            Thread.Sleep(200);

            var msgNr = messageCounter.NumberOfmessages;

            Console.WriteLine("number of messages " + msgNr);

            Assert.AreEqual(1 , msgNr);

        }

        public class MessageCounter
        {

            private UDPMulticastReceiver _receiver;
            public int NumberOfmessages { get; private set; }


            public MessageCounter(UDPMulticastReceiver receiver)
            {
                _receiver = receiver;
                NumberOfmessages = 1;
            }


            public void ListenAndCount() {
                _receiver.readMulticastGroupMessage();
                NumberOfmessages++;
            }


        }
*/


    }
}
