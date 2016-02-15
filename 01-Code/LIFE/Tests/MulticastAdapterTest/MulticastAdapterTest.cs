//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 25.01.2016
//  *******************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
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

			var testListenPort = 50000;
			var mcastAddress = "239.0.0.4";

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


            
            var listenThread = new Thread(new ThreadStart(
                messageCounter.ListenAndCount));
            listenThread.Start();

            foreach (var adapter in multicastAdapters)
            {
				adapter.SendMessageToMulticastGroup(new []{ new byte()});
            }

            Thread.Sleep(200);

            Assert.GreaterOrEqual(messageCounter.NumberOfmessages, numberOfMulticastComponents);
       

            messageCounter.StopRunning();
            
            foreach (var adapter in multicastAdapters)
            {
				adapter.SendMessageToMulticastGroup(new []{ new byte()});
            }

            Thread.Sleep(50);

            foreach (var adapter in multicastAdapters)
            {
                adapter.CloseSocket();
            }

            
        }


        /// <summary>
        ///     Test the udp multicast connection between the UDPMulticastReceiver and UDPMulticastSender.
        ///     Fails if:
        ///     - 0 Messages are received by the UDPMulticastReceiver.
        /// </summary>
        [Test]
        public void SendMessageTest()
        {
			var testListenPort = 6666;
            var testSendIngPortStartSeed = 6000;
			var mcastAddress = "239.0.0.2";

            var reciever = new UDPMulticastReceiver(IPAddress.Parse(mcastAddress), testListenPort);
            var sender =
                new UDPMulticastSender(
                    new GlobalConfig(mcastAddress, testListenPort, testSendIngPortStartSeed, 4),
                    new MulticastSenderConfig());

            var messageCounter = new MessageCounter(reciever);

            var listenThread = new Thread(messageCounter.ListenAndCount);
            listenThread.Start();

			sender.SendMessageToMulticastGroup(new []{ new byte()});

            //wait for message to arrive.
            Thread.Sleep(1000);

            var msgNr = messageCounter.NumberOfmessages;
            
            Assert.GreaterOrEqual(msgNr, 1);

            messageCounter.StopRunning();

			sender.SendMessageToMulticastGroup(new []{ new byte()});

            Thread.Sleep(50);

            sender.CloseSocket();
            reciever.CloseSocket();
        }

        [Test]
        public void UDPSenderShutDownTest() {
            var testListenPort = 50000;
            var testSendIngPortStartSeed = 60066;
            var mcastAddress = "224.50.50.50";

            var sender =
                new UDPMulticastSender(
                    new GlobalConfig(mcastAddress, testListenPort, testSendIngPortStartSeed, 4),
                    new MulticastSenderConfig());
            sender.CloseSocket();

            foreach (var socket in sender.GetSockets())
            {
                Assert.IsTrue(socket.Client == null);
            }   

        }

        [Test]
        public void UDPRecieverShutDownTest() 
        {
            var mcastAddress = "224.50.50.50";
			var testListenPort = 50000;
            var reciever = new UDPMulticastReceiver(IPAddress.Parse(mcastAddress), testListenPort);
            
            reciever.CloseSocket();

            Assert.IsTrue(reciever.GetSocket().Client == null);

        }

        #region Nested type: MessageCounter

        public class MessageCounter {

            private bool _stopRunning;

          
            public int NumberOfmessages { get; private set; }
            private readonly UDPMulticastReceiver _receiver;


            public MessageCounter(UDPMulticastReceiver receiver) {
                _stopRunning = false;
                _receiver = receiver;
                NumberOfmessages = 0;
              

            }

            public void StopRunning() {
                _stopRunning = true;
               
            }
           

            public void ListenAndCount()
            {
                while (!_stopRunning)
                {
                    _receiver.readMulticastGroupMessage();
                    NumberOfmessages++;
                }
            }
        }

        #endregion
    }
}