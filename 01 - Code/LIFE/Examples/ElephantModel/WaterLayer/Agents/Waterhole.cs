using System;
using CSharpQuadTree;
using LayerAPI.Interfaces;
using System.Windows;

namespace WaterLayer
{
	public class Waterhole: IAgent, IQuadObject
	{
		private double _capacity;

		public Waterhole (double x, double y, Size size)
		{
			_capacity = 100.0;
			Bounds.X = x;
			Bounds.Y = y;
			Bounds.Size = size;
		}

		public double Capacity { get { return _capacity; } }

		public double TakeWater(double amount){
			var res = _capacity - amount;
			if (res >= 0) {
				_capacity = res;
				return amount;
			} else {
				_capacity = 0;
				return amount - res;
			}
		}

		#region ITickClient implementation

		public void Tick ()
		{
			// just reset for now, infinite waterhole
			_capacity = 100;
		}

		#endregion

		#region IQuadObject implementation

		public event EventHandler BoundsChanged;

		public System.Windows.Rect Bounds {
			get ;
			private set;
		}

		#endregion
	}
}

