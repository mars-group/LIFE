using System;

namespace ContextServiceClient
{
	public class GazelleEvent
	{
		public GazelleEvent ()
		{
		}

		public GazelleEvent (int id, int x, int y, int energy)
		{
			this.id = id;
			this.x = x;
			this.y = y;
			this.energy = energy;
		}

		public int id {get;set;}
		public int x {get;set;}
		public int y {get;set;}
		public int energy {get;set;}
	}
}

