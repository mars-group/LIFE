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

		public double Capacity { get; private set; }
		public Rect Bounds { get; private set;}
	}
}

