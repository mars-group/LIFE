using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlantLayer.Agents;
using System.Windows;

namespace PlantLayer
{
    public class TPlant
    {
		private double _health;
		private Rect _bounds;

        public TPlant(Plant p)
        {
			_health = p.GetHealth ();
			_bounds = p.Bounds;
        }

		public double GetHealth(){
			return _health;
		}

		public Rect GetBounds(){
			return _bounds;
		}
    }
}
