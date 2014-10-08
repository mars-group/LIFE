using System;
using LayerAPI.Interfaces;

namespace CheetahModel
{
	public class Impala : Prey,IAgent
	{
		public Impala ()
		{
		}

		public void Tick(){
			Console.WriteLine ("I am a prey!");

		}
			
	}
}

