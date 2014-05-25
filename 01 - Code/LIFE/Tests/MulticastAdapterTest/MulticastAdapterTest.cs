using System;
using System.Net;
using System.Threading;
using AppSettingsManager;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Config.Types;
using NUnit.Framework;

namespace MulticastAdapterTest
{
  public class MulticastAdapterTest
    {


        [Test]
        public void TestPortIncrement()
        {
            var startListenPort = 50555;

            var globalConfig = new GlobalConfig("224.10.100.1", 60543, startListenPort, 4);
            var senderConfig = new MulticastSenderConfig();

            // if no exceptions are thrown we are good to go
			Assert.DoesNotThrow (() => new MulticastAdapterComponent (globalConfig, senderConfig));
			Assert.DoesNotThrow(() => new MulticastAdapterComponent(globalConfig, senderConfig));

        }
        
        [Test]
        public void SendExcaltyOneMessageTest()
        {

            var testListenPort = 60030;
            var testSendIngPortStartSeed = 60000;
            var mcastAddress = "224.50.50.50";

            var reciever = new UDPMulticastReceiver(IPAddress.Parse(mcastAddress), testListenPort);
            var sender =
                new UDPMulticastSender(new GlobalConfig(mcastAddress, testListenPort, testSendIngPortStartSeed, 4),
					new MulticastSenderConfig());

            var messageCounter  = new MessageCounter(reciever);

            var listenThread = new Thread(messageCounter.ListenAndCount);
            listenThread.Start();

            sender.SendMessageToMulticastGroup(new byte[0]);

            //wait for message to arrive.
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
                NumberOfmessages = 0;
            }


            public void ListenAndCount() {
                _receiver.readMulticastGroupMessage();
                NumberOfmessages++;
            }


        }



    }
}
