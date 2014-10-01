using System;
using System.Windows;

namespace WaterLayer
{
	public class TWaterhole
	{
		private double _capacity;
		private Rect _bounds;

		public TWaterhole (Waterhole w)
		{
			_capacity = w.Capacity;
			_bounds = w.Bounds;
		}

		public double Capacity { get; }
		public Rect Bounds { get; }
	}
}

