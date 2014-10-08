using System;
using CSharpQuadTree;
using LayerAPI.Interfaces;
using System.Windows;
using TwoDimEnvironment;

namespace CheetahModel
{
	public class Cheetah : Predator, IAgent
	{

		//bool Dead;
		private Rect _bounds;
		Gender G;
		State S;
		Prey p;
		ITwoDimEnvironment<Animal> environment;

		public Cheetah (int age, Gender g, float x, float y, ITwoDimEnvironment<Animal> env)
		{
			//Dead = false;
			this.Age = age;
			G = g;
			_bounds.X = x;
			_bounds.Y = y;
			this.environment = env;
		}


		public void Tick ()
		{
//			double x = _bounds.X + 1;
//			double y = _bounds.Y + 1;
//			environment.Move (this, x, y);

			//this.MoveTo (x, y);
//			if (p == null) {
//				S = State.Searching;
//				p = SearchForPrey ();
//			} else {
//			}
//			S = State.Stalking;
//			StalkPrey (p);
//			if (Environment.DistanceToAgent (this, p) < 10) {
//				S = State.Chasing;
//				ChasePrey (p);
//
//			}

			Console.WriteLine ("Agent: " + this + " " + G + " Position:" + _bounds.X + " " + _bounds.Y);

		}

		public Rect Get_bounds ()
		{
			return _bounds;
		}

		public ITwoDimEnvironment<Animal> GetEnvironment ()
		{
			return environment;
		}

	}

}