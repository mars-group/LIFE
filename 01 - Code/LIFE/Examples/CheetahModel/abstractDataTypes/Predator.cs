using System;

namespace CheetahModel
{
	public abstract class Predator:Animal
	{

		public enum State{Searching, Stalking, Chasing, Resting}

	
		public Prey SearchForPrey ()
		{
			//Beutetiere in Umgebung abfragen und dichtestes zurückgeben
			Impala p = new Impala ();
			return p;
		}

		public void StalkPrey (Prey prey)
		{
		}

		public void ChasePrey (Prey prey)
		{
		}

		public void KillPrey (Prey prey)
		{
		}

		public void Rest ()
		{
		}

		public void EatPrey (Prey prey)
		{
			int nutri = prey.NutritionalValue;
			this.SaturationLevel = this.SaturationLevel + nutri;
		}

//		public bool MoveTo ()
//		{
//			return true;
//		}

		public void FollowPreyAtDistance ()
		{
		
		}

		private void Accelerate (int x)
		{
		}

		private void Deccelerate (int x)
		{
		}
	}
}