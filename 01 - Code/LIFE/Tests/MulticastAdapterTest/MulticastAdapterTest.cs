using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using AppSettingsManager;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;
using NUnit.Framework;

namespace MulticastAdapterTest
{
    public class MulticastAdapterTest
    {
        /// <summary>
        ///     Test if more than one MulticastAdapter can be initalizes and receaves messages
        /// </summary>
        [Test]
        public void TestPortIncrement()
        {
            int numberOfMulticastComponents = 10;

            var testListenPort = 60035;
            var mcastAddress = "224.10.100.1";

            var startListenPort = 50555;

            var globalConfig = new GlobalConfig(mcastAddress, testListenPort, startListenPort, 4);
            var senderConfig = new MulticastSenderConfig();

            IList<MulticastAdapterComponent> multicastAdapters = new List<MulticastAdapterComponent>();

            MulticastAdapterComponent mcastAdapterComp = null;
            for (int i = 0; i < numberOfMulticastComponents; i++)
            {
                Assert.DoesNotThrow(() => mcastAdapterComp = new MulticastAdapterComponent(globalConfig, senderConfig));
                multicastAdapters.Add(mcastAdapterComp);
            }

            Assert.AreEqual(numberOfMulticastComponents, multicastAdapters.Count);

            var reciever = new UDPMulticastReceiver(IPAddress.Parse(mcastAddress), testListenPort);

            var messageCounter = new MessageCounter(reciever);

            var listenThread = new Thread(
                messageCounter.ListenAndCount);
            listenThread.Start();

            foreach (var adapter in multicastAdapters)
            {
                adapter.SendMessageToMulticastGroup(new byte[0]);
            }

            Thread.Sleep(200);

            Assert.GreaterOrEqual(messageCounter.NumberOfmessages, numberOfMulticastComponents);

            listenThread.Interrupt();
        }


        /// <summary>
        ///     Test the udp multicast connection between the UDPMulticastReceiver and UDPMulticastSender.
        ///     Fails if:
        ///     - 0 Messages are received by the UDPMulticastReceiver.
        /// </summary>
        [Test]
        public void SendMessageTest()
        {
            var testListenPort = 60030;
            var testSendIngPortStartSeed = 60000;
            var mcastAddress = "224.50.50.50";

            var reciever = new UDPMulticastReceiver(IPAddress.Parse(mcastAddress), testListenPort);
            var sender =
                new UDPMulticastSender(
                    new GlobalConfig(mcastAddress, testListenPort, testSendIngPortStartSeed, 4),
                    new MulticastSenderConfig());

            var messageCounter = new MessageCounter(reciever);

            var listenThread = new Thread(messageCounter.ListenAndCount);
            listenThread.Start();

            sender.SendMessageToMulticastGroup(new byte[0]);

            //wait for message to arrive.
            Thread.Sleep(200);

            var msgNr = messageCounter.NumberOfmessages;
            
            Assert.GreaterOrEqual(msgNr, 1);

            listenThread.Interrupt();
        }

        #region Nested type: MessageCounter

        public class MessageCounter
        {
            public int NumberOfmessages { get; private set; }
            private readonly UDPMulticastReceiver _receiver;


            public MessageCounter(UDPMulticastReceiver receiver)
            {
                _receiver = receiver;
                NumberOfmessages = 0;
            }


            public void ListenAndCount()
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    _receiver.readMulticastGroupMessage();
                    NumberOfmessages++;
                }
            }
        }

        #endregion
    }
}