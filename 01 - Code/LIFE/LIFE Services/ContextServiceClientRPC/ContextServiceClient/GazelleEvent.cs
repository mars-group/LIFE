using System;

namespace ContextServiceClient
{
	public class GazelleEvent
	{
		public GazelleEvent ()
		{
		}

		public GazelleEvent (int id, double x, string y, bool energy)
		{
			this.id = id;
			this.x = x;
			this.y = y;
			this.energy = energy;
		}

		public int id {get;set;}
		public double x {get;set;}
		public string y {get;set;}
		public bool energy {get;set;}
	}
}

