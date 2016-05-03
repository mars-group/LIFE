//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 26.01.2016
//  *******************************************************/
using System;
using System.Collections.Generic;
using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Server;
using ASC.Communication.ScsServices.Client;
using ASC.Communication.ScsServices.Service;
using System.Threading.Tasks;
using LIFEUtilities.MulticastAddressGenerator;
using System.Diagnostics;
using AgentShadowingService.Implementation;
using System.Linq;
using System.Collections.Concurrent;
using ASC.Communication.Scs.Communication.Messages;
using ASC.Communication.ScsServices.Communication.Messages;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ASC.Communication.Scs.Communication.Channels.Udp;
using System.Net;
using ASC.Communication.Scs.Communication.EndPoints.Udp;

namespace PerfTester
{
	class MainClass
	{
		static int rcvd = 0;
		static int AgentsPerNode = 100;
		public static void Main (string[] args)
		{
			if (args != null){
				foreach (var s in args) {
					rcvd = 0;
					AgentsPerNode = int.Parse(s);
					Console.WriteLine($"Using {AgentsPerNode*2} agents");
					//TestLocalFirstResolution();
					//TestBinarySerializer();
					TestAsyncSocketEventArgs();
					//TestCommunicationPerformance();
				}

			}

		}

		public static void TestLocalFirstResolution()
		{
			Console.WriteLine("Now testing local first resolution performance!");
			// create and register RealAgents in serviceA+B
			var _agentsA = new List<MockAgent>();
			var _agentsB = new List<MockAgent>();

			var sw = Stopwatch.StartNew();
			var _concA = new ConcurrentDictionary<MockAgent, byte>();
			var _concB = new ConcurrentDictionary<MockAgent, byte>();
			Parallel.For(0, AgentsPerNode, i => {
				_concA.TryAdd(new MockAgent(), new byte());
				_concB.TryAdd(new MockAgent(), new byte());
			});
			_agentsA.AddRange(_concA.Keys);
			_agentsB.AddRange(_concA.Keys);

			Console.WriteLine($"Agent creation took: {sw.ElapsedMilliseconds}ms");
			sw.Restart();
			var serviceA = new AgentShadowingServiceComponent<IMockAgent, MockAgent>();
			var serviceB = new AgentShadowingServiceComponent<IMockAgent, MockAgent>();

			serviceA.RegisterRealAgents(_agentsA.ToArray());
			serviceB.RegisterRealAgents(_agentsB.ToArray());

			Console.WriteLine($"Agent registration took: {sw.ElapsedMilliseconds}ms");
			sw.Restart();
			// create Shadowagents
			var shadows = new List<IMockAgent>();
			shadows.AddRange(serviceA.ResolveAgents(_agentsB.Select(a => a.ID).ToArray()));
			shadows.AddRange(serviceB.ResolveAgents(_agentsA.Select(a => a.ID).ToArray()));
			Console.WriteLine($"Agent resolution took: {sw.ElapsedMilliseconds}ms");
			sw.Restart();
			Parallel.ForEach(shadows, mockAgent => mockAgent.DoCrazyShit());

			sw.Stop();
			Console.WriteLine($"Calling methods took: {sw.ElapsedMilliseconds}ms");
			serviceA.Dispose();
			serviceB.Dispose();

		}

		public static void TestCommunicationPerformance() {
			Console.WriteLine("Now testing communication performance!");
			// create and register RealAgents in serviceA+B
			var _agentsA = new List<MockAgent>();
			var _agentsB = new List<MockAgent>();

			var sw = Stopwatch.StartNew();
			var _concA = new ConcurrentDictionary<MockAgent, byte>();
			var _concB = new ConcurrentDictionary<MockAgent, byte>();
			Parallel.For(0, AgentsPerNode, i =>
			{
				_concA.TryAdd(new MockAgent(), new byte());
				_concB.TryAdd(new MockAgent(), new byte());
			});
			_agentsA.AddRange(_concA.Keys);
			_agentsB.AddRange(_concA.Keys);
			Console.WriteLine($"Agent creation took: {sw.ElapsedMilliseconds}ms");
			sw.Restart();

			var port = 6666;
			var clientListenPort = port;
			var typeOfTServiceClass = typeof(MockAgent);
			var typeOfTServiceInterface = typeof(IMockAgent);

			var mcastAddress = MulticastAddressGenerator.GetIPv4MulticastAddressByType(typeOfTServiceClass);

			var ascServiceApp1 = new AscServiceApplication(AcsServerFactory.CreateServer(AscEndPoint.CreateEndPoint(port, mcastAddress)));
			var ascServiceApp2 = new AscServiceApplication(AcsServerFactory.CreateServer(AscEndPoint.CreateEndPoint(port, mcastAddress)));
			ascServiceApp1.Start();
			ascServiceApp2.Start();
			Console.WriteLine($"AgentShadowing service creation took: {sw.ElapsedMilliseconds}ms");
			sw.Restart();

			Parallel.ForEach(_agentsA, a =>
			{
				ascServiceApp1.AddService<IMockAgent, MockAgent>(a, typeOfTServiceInterface);
			});

			Parallel.ForEach(_agentsB, b =>
			{
				ascServiceApp2.AddService<IMockAgent, MockAgent>(b, typeOfTServiceInterface);
			});

			Console.WriteLine($"Agent registration took: {sw.ElapsedMilliseconds}ms");
			sw.Restart();

			// create shadows for A and B
			var shadows = new ConcurrentBag<IMockAgent>();
			Parallel.ForEach(_agentsA, a =>
				{
					var shadow = AscServiceClientBuilder.CreateClient<IMockAgent>(
						clientListenPort,
						mcastAddress,
						a.ID
					);
					shadow.Timeout = -1;
					shadow.Connect();
					shadows.Add(shadow.ServiceProxy);
				});

			Parallel.ForEach(_agentsB, b =>
				{
					var shadow = AscServiceClientBuilder.CreateClient<IMockAgent>(
						clientListenPort,
						mcastAddress,
						b.ID
					);
					shadow.Timeout = -1;
					shadow.Connect();
					shadows.Add(shadow.ServiceProxy);
				});

			Console.WriteLine($"Agent resolution took: {sw.ElapsedMilliseconds}ms");
			sw.Restart();

			// call the method on all shadows
			Parallel.ForEach(shadows, mockAgent => mockAgent.DoCrazyShit());

			sw.Stop();
			Console.WriteLine($"Calling methods took : {sw.ElapsedMilliseconds} ms");

			ascServiceApp1.Stop();
			ascServiceApp2.Stop();
		}

		public static void TestBinarySerializer() {
			var bin = new BinaryFormatter();
			Console.WriteLine("Now testing BinaryFormatter!");
			var messages = new ConcurrentBag<IAscMessage>();
			Parallel.For(0, AgentsPerNode * 2, i => messages.Add(new AscRemoteInvokeMessage()
			{
				ServiceInterfaceName = "IAscMessage",
				MethodName = "DoSomething",
				ServiceID = Guid.NewGuid()
			}));

			var sw = Stopwatch.StartNew();
			Parallel.ForEach(messages, m =>
			{
				var memoryStream = new MemoryStream();

				bin.Serialize(memoryStream, m);

				var messageBytes = memoryStream.ToArray();
			});
			sw.Stop();
			Console.WriteLine($"BinaryFormatter: {sw.ElapsedMilliseconds}ms");
		}

		public static void TestAsyncSocketEventArgs() {
			var bin = new BinaryFormatter();
			Console.WriteLine("Now testing Message creation!");
			var sw = Stopwatch.StartNew();
			var messages = new ConcurrentBag<IAscMessage>();

			Parallel.For(0, AgentsPerNode * 2, i => messages.Add(new AscRemoteInvokeMessage()
			{
				ServiceInterfaceName = "IAscMessage",
				MethodName = "DoSomething",
				ServiceID = Guid.NewGuid()
			}));

			var msgCreation = sw.ElapsedMilliseconds;
			Console.WriteLine($"Message creation: {msgCreation}ms");
			sw.Restart();

			Console.WriteLine("Now testing BinaryFormatter!");
			var messageBytes = new ConcurrentBag<byte[]>();

			Parallel.ForEach(messages, m =>
			{
				var memoryStream = new MemoryStream();

				bin.Serialize(memoryStream, m);

				messageBytes.Add(memoryStream.ToArray());
			});

			var binForm = sw.ElapsedMilliseconds;
			Console.WriteLine($"BinaryFormatter: {binForm}ms");

			var msgsAry = messageBytes.ToArray();
			sw.Restart();

			Console.WriteLine("Now testing UdpAsyncSocketEventArgs!");

			var server = new UdpAsyncSocketServer(2,12,8000);
			server.Init();
			server.DatagramReceived += On_DatagramReceived;
			var endPoint = new AscUdpEndPoint(6666, "239.10.11.12");
			server.Start(new IPEndPoint(IPAddress.Any, endPoint.UdpPort), IPAddress.Parse(endPoint.McastGroup));

			/*for (int i = 0; i < msgsAry.Length; i++) {
				server.Send(msgsAry[0]);
			}*/
			Parallel.ForEach(messageBytes, b => server.Send(b));

			sw.Stop();
			var udp = sw.ElapsedMilliseconds;
			Console.WriteLine($"UDP Send: {udp}ms\n, rcvd: {rcvd}");
			Console.WriteLine($"Total duration: {udp+binForm+msgCreation}ms.\n\n");
		}

		static void On_DatagramReceived(object sender, byte[] e)
		{
			rcvd++;
		}
}

	internal class MockAgent : AscService, IMockAgent {

		public MockAgent(){
			ID = Guid.NewGuid();
			ServiceID = ID;
		}

		public Guid ID { get; private set;}

		public int DoCrazyShit() {
			return 42;
		}
	}

	[AscService(Version = "0.1")]
	internal interface IMockAgent {
		int DoCrazyShit();
	}
}
