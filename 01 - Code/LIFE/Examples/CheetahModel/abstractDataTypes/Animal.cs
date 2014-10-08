using System;
using CSharpQuadTree;
using LayerAPI.Interfaces;
using System.Windows;
using TwoDimEnvironment;

namespace CheetahModel
{
	public abstract class Animal : IQuadObject
	{
		#region IQuadObject implementation

		public event EventHandler BoundsChanged;

		public Rect Bounds {
			get {
				return _bounds;
			}
			set {
				_bounds = value;
			}
		}

		#endregion


		public int Age{ get; set;}
		public enum Gender {Male, Female}
		public bool Hunger{ get; set;}
		public bool Thirst{ get; set;}
		public int SaturationLevel{ get; set;} // berechnet sich aus Masse und Größe
		public int Velocity{ get; set;}
		public int Size{ get; set;}
		public int Mass{ get; set;}
		private Rect _bounds;


		public int getSaturationLevel(){
			int s = this.Size * this.Mass;
			return s;
		}

		public bool MoveTo (double x, double y)
		{
			return false;
		}
}
}