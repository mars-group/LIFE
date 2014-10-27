using System;

namespace ContextServiceClient
{
	public class GepardEvent
	{
		public GepardEvent ()
		{
		}

		public GepardEvent (int id, int x, int y, bool male, int count, int age)
		{
			this.id = id;
			this.x = x;
			this.y = y;
			this.male = male;
			this.count = count;
			this.age = age;
		}

		public int id {get;set;}
		public int x {get;set;}
		public int y {get;set;}
		public bool male {get;set;}
		public int count {get;set;}
		public int age {get;set;}
	}
}

