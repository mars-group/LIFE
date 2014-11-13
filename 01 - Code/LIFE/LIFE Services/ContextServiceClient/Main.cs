﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContextServiceClient
{
	class Program
	{

		public static void Main(string[] args)
		{
			ContextServiceClient csc = ContextServiceClient.Instance;
			csc.ConnectTo("141.22.29.51", 5672, "GGSimulation");

			csc.RegisterNewEventType(new GazelleEvent());
			csc.RegisterNewEventType(new GepardEvent());
			csc.RegisterNewEventType(new KillEvent());

			csc.RegisterNewContextRule("select e1, e2 from pattern [every (e1=KillEvent(x < 30, y < 30) -> e2=KillEvent(x > 30, y > 30)) where timer:within(10 sec)]", Listener.MethodOne);
			csc.RegisterNewContextRule("select id, energy from GazelleEvent", Listener.MethodTwo);
			csc.RegisterNewContextRule("select count(*) from pattern [every (timer:interval(20 sec) and not ([5] KillEvent))]", Listener.MethodThree);
			csc.RegisterNewContextRule("select e1, e2 from pattern [every (e1=GazelleEvent -> e2=GazelleEvent(e1.x = e2.x, e1.y = e2.y)) where timer:within(5 sec)]", Listener.CollisionDetectionMethod);
			//csc.RegisterNewContextRule("select * from GazelleEvent where id between 1 and 10", Listener.MethodTwo);

			EventProducer eventProducer = new EventProducer();

			GazelleEvent gazelleEvent1 = new GazelleEvent(2, 23, 78, 11);
			GazelleEvent gazelleEvent2 = new GazelleEvent(34, 23, 78, 14);

			KillEvent killEvent1 = new KillEvent(3, 9, 22, 17);
			KillEvent killEvent2 = new KillEvent(3, 9, 43, 67);

			eventProducer.SendEvent(gazelleEvent1);
			eventProducer.SendEvent(gazelleEvent2);
			eventProducer.SendEvent(killEvent1);
			eventProducer.SendEvent(killEvent2);
		}
	}

	class Listener {
		public static void MethodOne(Dictionary<string, object> results) {

			Dictionary<string, object> e1 = JsonConvert
				.DeserializeObject<Dictionary<string, object>>((string)results["e1"]);
			Dictionary<string, object> e2 = JsonConvert
				.DeserializeObject<Dictionary<string, object>>((string)results["e2"]);

			Console.WriteLine("Detected KillEvent at position ({0},{1}) followed by KillEvent at position ({2},{3}) within 5 seconds", e1["x"], e1["y"], e2["x"], e2["y"]);

		}

		public static void MethodTwo(Dictionary<string, object> results) {

			Console.WriteLine("Received GazelleEvent from gazelle with id: {0} and energy: {1}", results["id"], results["energy"]);
			// Do something
		}

		public static void MethodThree(Dictionary<string, object> results) {

			Console.WriteLine("Fewer than five KillEvents in the last 20 seconds");
			// Do something
		}

		public static void CollisionDetectionMethod(Dictionary<string, object> results) {

			Dictionary<string, object> e1 = JsonConvert
				.DeserializeObject<Dictionary<string, object>>((string)results["e1"]);
			Dictionary<string, object> e2 = JsonConvert
				.DeserializeObject<Dictionary<string, object>>((string)results["e2"]);

			Console.WriteLine("Gazelle {0} and gazelle {1} collided at position ({2},{3})", e1["id"], e2["id"], e1["x"], e1["y"]);
		
			// Do something
		}
	}
}
