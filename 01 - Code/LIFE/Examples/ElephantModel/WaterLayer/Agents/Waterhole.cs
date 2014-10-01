using System;
using CSharpQuadTree;
using LayerAPI.Interfaces;

namespace WaterLayer
{
	public class Waterhole: IAgent, IQuadObject
	{
		private double _capacity;

		public Waterhole ()
		{
			_capacity = 100.0;
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
	}
}

