using System;

namespace ContextServiceClient
{
	public class KillEvent
	{
		public KillEvent ()
		{
		}

		public KillEvent (int gepardId, int gazelleId, int x, int y)
		{
			this.gepardId = gepardId;
			this.gazelleId = gazelleId;
			this.x = x;
			this.y = y;
		}

		public int gepardId {get;set;}
		public int gazelleId {get;set;}
		public int x {get;set;}
		public int y {get;set;}
	}
}

