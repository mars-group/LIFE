using System;
using System.Collections.Generic;
using System.Reflection;

namespace ContextServiceClient
{
	class Program
	{

		public static void Main(string[] args)
		{
			GazelleEvent gazelle = new GazelleEvent(2, 5.3, "78", true);

			ContextServiceClient csc = new ContextServiceClient ();
			csc.RegisterNewEventType (gazelle);
			//csc.RegisterNewEventType (new GepardEvent());
			csc.RegisterNewContextRule ("select * from GazelleEvent", Demo.SayHello);
			csc.RegisterNewContextRule ("select * from GazelleEvent where id between 8 and 10", Demo.SayBye);

			csc.SendEvent(gazelle);




			while(true){}
		}
	}

	class Demo {
		public static void SayHello() {
			Console.WriteLine("Hello");
		}
		public static void SayBye() {
			Console.WriteLine("Bye");
		}
	}
}

