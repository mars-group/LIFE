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

namespace PerfTester
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var AgentsPerNode = 10000;
			// create and register RealAgents in serviceA
			var _agentsA = new List<MockAgent>();
			for (var i = 0; i < AgentsPerNode; i++)
			{
				_agentsA.Add(new MockAgent());
			}

			// create and register RealAgents in serviceB
			var _agentsB = new List<MockAgent>();
			for (var i = 0; i < AgentsPerNode; i++)
			{
				_agentsB.Add(new MockAgent());
			}

			var port = 6666;
			var clientListenPort = port;
			var typeOfTServiceClass = typeof(MockAgent);
			var typeOfTServiceInterface = typeof(IMockAgent);

			var mcastAddress = MulticastAddressGenerator.GetIPv4MulticastAddressByType(typeOfTServiceClass);

			var ascServiceApp1 = new AscServiceApplication(AcsServerFactory.CreateServer(AscEndPoint.CreateEndPoint(port, mcastAddress)));
			var ascServiceApp2 = new AscServiceApplication(AcsServerFactory.CreateServer(AscEndPoint.CreateEndPoint(port, mcastAddress)));
			ascServiceApp1.Start();
			ascServiceApp2.Start();


			// create shadows for A and B
			var shadows = new List<IMockAgent>();
			_agentsA.ForEach(a =>
				{
					ascServiceApp1.AddService<IMockAgent, MockAgent>(a, typeOfTServiceInterface);
					var shadow = AscServiceClientBuilder.CreateClient<IMockAgent>(
						clientListenPort,
						mcastAddress,
						a.ID
					);
					shadow.Timeout = -1;
					shadow.Connect();
					shadows.Add(shadow.ServiceProxy);
				});

			_agentsB.ForEach(b =>
				{
					ascServiceApp2.AddService<IMockAgent, MockAgent>(b, typeOfTServiceInterface);
					var shadow = AscServiceClientBuilder.CreateClient<IMockAgent>(
						clientListenPort,
						mcastAddress,
						b.ID
					);
					shadow.Timeout = -1;
					shadow.Connect();
					shadows.Add(shadow.ServiceProxy);
				});

			var sw = Stopwatch.StartNew();
			// call the method on all shadows
			Parallel.ForEach(shadows, mockAgent => mockAgent.DoCrazyShit());

			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			ascServiceApp1.Stop();
			ascServiceApp2.Stop();
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
